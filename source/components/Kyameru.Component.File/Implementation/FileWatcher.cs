using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kyameru.Component.File.Utilities;
using Kyameru.Core.Entities;

using Microsoft.Extensions.Logging;

namespace Kyameru.Component.File
{
    /// <summary>
    /// From component.
    /// </summary>
    public class FileWatcher : IFileWatcher
    {
        /// <summary>
        /// List of delegates to perform actions.
        /// </summary>
        private readonly Dictionary<string, Action> fswSetup = new Dictionary<string, Action>();

        /// <summary>
        /// Valid headers.
        /// </summary>
        private readonly Dictionary<string, string> config;

        /// <summary>
        /// List of directories to ignore.
        /// </summary>
        private readonly string[] directoriesToIgnore;

        /// <summary>
        /// List of strings that should be ignored as part of the file name and extension.
        /// </summary>
        private readonly string[] stringsToIgnore;

        /// <summary>
        /// Value indicating whether an initial scan will occur of the directory.
        /// </summary>
        private bool willScan = false;

        /// <summary>
        /// File system watcher.
        /// </summary>
        private IFileSystemWatcher fsw;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcher"/> class.
        /// </summary>
        /// <param name="headers">Incoming Headers.</param>
        public FileWatcher(Dictionary<string, string> headers, IFileSystemWatcher fileSystemWatcher)
        {
            this.config = headers.ToFromConfig();
            this.SetupInternalActions();
            this.Setup();
            this.fsw = fileSystemWatcher;
            this.DetermineScan(this.config["InitialScan"]);
            this.directoriesToIgnore = this.config["Ignore"].SplitPiped();
            this.stringsToIgnore = this.config["IgnoreStrings"].SplitPiped();
        }

        /// <summary>
        /// Event raised when file picked up.
        /// </summary>
        public event EventHandler<Routable> OnAction;

        /// <summary>
        /// Event raised when needing to log.
        /// </summary>
        public event EventHandler<Log> OnLog;

        /// <summary>
        /// Sets up the component.
        /// </summary>
        public void Setup()
        {
            this.VerifyArguments();
        }

        /// <summary>
        /// Starts the component.
        /// </summary>
        public void Start()
        {
            this.SetupFsw();
            this.ScanFiles();
            this.SetupSubDirectories();
            this.fsw.EnableRaisingEvents = true;
            this.fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            foreach (string item in this.config["Notifications"].Split(','))
            {
                if (this.fswSetup.ContainsKey(item))
                {
                    this.fswSetup[item]();
                }
            }
        }

        /// <summary>
        /// Stops the component.
        /// </summary>
        public void Stop()
        {
            this.fsw.Dispose();
        }

        /// <summary>
        /// Sets up the file watcher.
        /// </summary>
        /// <returns>Returns a new <see cref="FileSystemWatcher"/>.</returns>
        private void SetupFsw()
        {
            this.fsw.Path = this.config["Target"];
            this.fsw.Filter = this.config["Filter"];
        }

        /// <summary>
        /// Sets up sub directories.
        /// </summary>
        private void SetupSubDirectories()
        {
            // Optional but always supplied.
            this.fsw.IncludeSubdirectories = bool.Parse(this.config["SubDirectories"]);
        }

        /// <summary>
        /// Abstraction for logging event.
        /// </summary>
        /// <param name="logLevel">Log level to raise.</param>
        /// <param name="message">Log message.</param>
        /// <param name="exception">Log exception.</param>
        private void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }

        /// <summary>
        /// Verifies the setup.
        /// </summary>
        private void VerifyArguments()
        {
            if (this.IsConfigBlank("Target"))
            {
                this.Log(LogLevel.Error, Resources.ERROR_TARGET_UNSPECIFIED);
                throw new ArgumentException(Resources.ERROR_TARGET_UNSPECIFIED);
            }

            if (this.IsConfigBlank("Notifications"))
            {
                this.Log(LogLevel.Error, Resources.ERROR_NOTIFICATIONS_UNSPECIFIED);
                throw new ArgumentException(Resources.ERROR_NOTIFICATIONS_UNSPECIFIED);
            }
        }

        private bool IsConfigBlank(string key)
        {
            return !this.config.ContainsKey(key) || string.IsNullOrWhiteSpace(this.config[key]);
        }

        /// <summary>
        /// Sets up internal delegates.
        /// </summary>
        private void SetupInternalActions()
        {
            this.fswSetup.Add("Created", this.AddCreated);
            this.fswSetup.Add("Changed", this.AddChanged);
            this.fswSetup.Add("Renamed", this.AddRenamed);
        }

        /// <summary>
        /// Add renamed event.
        /// </summary>
        private void AddRenamed()
        {
            this.fsw.Renamed += this.Fsw_Renamed;
        }

        /// <summary>
        /// Raised when file is renamed.
        /// </summary>
        /// <param name="sender">Class sending event.</param>
        /// <param name="e">Event arguments.</param>
        private void Fsw_Renamed(object sender, RenamedEventArgs e)
        {
            this.CreateMessage("Rename", e.FullPath);
        }

        /// <summary>
        /// Adds changed event.
        /// </summary>
        private void AddChanged()
        {
            this.fsw.Changed += this.Fsw_Changed;
        }

        /// <summary>
        /// Raised when file is changed.
        /// </summary>
        /// <param name="sender">Class sending event.</param>
        /// <param name="e">Event arguments.</param>
        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            this.CreateMessage("Changed", e.FullPath);
        }

        /// <summary>
        /// Adds created event.
        /// </summary>
        private void AddCreated()
        {
            this.fsw.Created += this.Fsw_Created;
        }

        /// <summary>
        /// Raised when file is created.
        /// </summary>
        /// <param name="sender">Class sending event.</param>
        /// <param name="e">Event arguments.</param>
        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            this.CreateMessage("Created", e.FullPath);
        }

        /// <summary>
        /// Creates a message to start the process.
        /// </summary>
        /// <param name="method">Method used to raise event.</param>
        /// <param name="sourceFile">Source file found.</param>
        private void CreateMessage(string method, string sourceFile)
        {
            try
            {
                if (this.IsProcessable(sourceFile))
                {
                    FileInfo info = new FileInfo(sourceFile);
                    sourceFile = sourceFile.Replace("\\", "/");
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("&SourceDirectory", System.IO.Path.GetDirectoryName(sourceFile));
                    headers.Add("SourceFile", System.IO.Path.GetFileName(sourceFile));
                    headers.Add("&FullSource", sourceFile);
                    headers.Add("&DateCreated", info.CreationTimeUtc.ToLongDateString());
                    headers.Add("Readonly", info.IsReadOnly.ToString());
                    headers.Add("Method", method);
                    headers.Add("DataType", "Byte");
                    Routable dataItem = new Routable(headers, System.IO.File.ReadAllBytes(sourceFile));
                    this.OnAction?.Invoke(this, dataItem);
                }
            }
            catch (Exception ex)
            {
                throw new Kyameru.Core.Exceptions.ComponentException("Error creating message, see inner exception.", ex);
            }
        }

        /// <summary>
        /// Determine if we should scan the directory.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private void DetermineScan(string header)
        {
            if (bool.TryParse(header, out this.willScan))
            {
                this.Log(LogLevel.Debug, "Scanning directory Enabled");
            }
        }

        private void ScanFiles()
        {
            this.Log(LogLevel.Debug, "Scanning directories");
            SearchOption searchOption = SearchOption.AllDirectories;
            if (!bool.Parse(this.config["SubDirectories"]))
            {
                searchOption = SearchOption.TopDirectoryOnly;
            }
            string[] files = Directory.GetFiles(this.config["Target"], this.config["Filter"], searchOption);
            if (files.Any())
            {
                for (int i = 0; i < files.Count(); i++)
                {
                    this.Log(LogLevel.Information, $"Checking file {files[i]}");
                    this.CreateMessage("Scanned", files[i]);
                }
            }
        }

        private bool IsProcessable(string file)
        {
            bool response = false;
            if (System.IO.File.Exists(file))
            {
                response = true;
                string toCheck = file.ToLower();

                if (this.ContainsIgnoreDirectories(file) || this.stringsToIgnore.Any(x => toCheck.Contains(x)))
                {
                    response = false;
                }
            }

            return response;
        }

        private bool ContainsIgnoreDirectories(string file)
        {
            string[] folders = file.ToLower().Split(Path.DirectorySeparatorChar);
            return folders.Select(x => x).Intersect(this.directoriesToIgnore).Any();
        }
    }
}