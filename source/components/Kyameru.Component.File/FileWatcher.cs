using System;
using System.Collections.Generic;
using System.IO;
using Kyameru.Core.Entities;

using Microsoft.Extensions.Logging;

namespace Kyameru.Component.File
{
    public class FileWatcher : Kyameru.Core.Contracts.IFromComponent
    {
        public event EventHandler<Routable> OnAction;

        public event EventHandler<Log> OnLog;

        private FileSystemWatcher fsw;

        private readonly Dictionary<string, Action> fswSetup = new Dictionary<string, Action>();
        private readonly Dictionary<string, string> config;

        public FileWatcher(string[] args)
        {
            this.config = args.ToFromConfig();
            this.SetupInternalActions();
        }

        public FileWatcher(Dictionary<string, string> headers)
        {
            this.config = headers.ToFromConfig();
            this.SetupInternalActions();
        }

        public void Setup()
        {
            this.VerifyArguments();
        }

        public void Start()
        {
            this.fsw = this.SetupFsw();

            this.SetupSubDirectories();
            this.fsw.EnableRaisingEvents = true;
            this.fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            foreach (string item in this.config["Notifications"].Split(','))
            {
                if (this.fswSetup.ContainsKey(item))
                {
                    this.fswSetup[item]();
                }
            }
        }

        private FileSystemWatcher SetupFsw()
        {
            return new FileSystemWatcher(this.config["Target"], this.config["Filter"]);
        }

        private void SetupSubDirectories()
        {
            if (this.config.Keys.Count == 4)
            {
                this.fsw.IncludeSubdirectories = bool.Parse(this.config["SubDirectories"]);
            }
        }

        public void Stop()
        {
            this.fsw.Dispose();
        }

        private void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }

        private void VerifyArguments()
        {
            if (string.IsNullOrWhiteSpace(this.config["Target"]))
            {
                throw new ArgumentException(Resources.ERROR_EXPECTEDSINGLE, "Target");
            }

            if (this.config.Keys.Count < 3)
            {
                throw new ArgumentException(Resources.ERROR_NOTENOUGHARGUMENTS_DIRECTORY);
            }
        }

        private void SetupInternalActions()
        {
            this.fswSetup.Add("Created", this.AddCreated);
            this.fswSetup.Add("Changed", this.AddChanged);
            this.fswSetup.Add("Renamed", this.AddRenamed);
        }

        private void AddRenamed()
        {
            this.fsw.Renamed += Fsw_Renamed;
        }

        private void Fsw_Renamed(object sender, RenamedEventArgs e)
        {
            //if (e.Name == this.fileName)
            //{
            this.CreateMessage("Rename", e.FullPath);
            //}
        }

        private void AddChanged()
        {
            this.fsw.Changed += Fsw_Changed;
        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            this.CreateMessage("Changed", e.FullPath);
        }

        private void AddCreated()
        {
            this.fsw.Created += Fsw_Created;
        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            this.CreateMessage("Created", e.FullPath);
        }

        private void CreateMessage(string method, string sourceFile)
        {
            FileInfo info = new FileInfo(sourceFile);
            sourceFile = sourceFile.Replace("\\", "/");
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("SourceDirectory", System.IO.Path.GetDirectoryName(sourceFile));
            headers.Add("SourceFile", System.IO.Path.GetFileName(sourceFile));
            headers.Add("FullSource", sourceFile);
            headers.Add("DateCreated", info.CreationTimeUtc.ToLongTimeString());
            headers.Add("Readonly", info.IsReadOnly.ToString());
            headers.Add("DataType", "byte");
            Routable dataItem = new Routable(headers, System.IO.File.ReadAllBytes(sourceFile));
            this.OnAction?.Invoke(this, dataItem);
        }

        public void SetError(Routable routable)
        {
            routable.SetInError("FromFile");
        }
    }
}