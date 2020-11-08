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

        private string fileName;
        private FileSystemWatcher fsw;
        private string[] fswArgs;
        private bool isDirectoryWatcher = false;

        private readonly Dictionary<string, Action> fswSetup = new Dictionary<string, Action>();

        public FileWatcher(string[] args)
        {
            this.SetupInternalActions();
            this.fswArgs = args;
        }

        public void Process(Routable item)
        {
            throw new Core.Exceptions.ProcessException(Resources.ERROR_MUSTSPECIFYPROCESSARGS);
        }

        public void Setup()
        {
            this.VerifyArguments(this.fswArgs);
            FileAttributes fileAttributes = System.IO.File.GetAttributes(this.fswArgs[0]);
            if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                this.isDirectoryWatcher = true;
            }

            this.fileName = this.fswArgs[0];
        }

        public void Start()
        {
            this.fsw = this.SetupFsw();

            this.SetupSubDirectories();
            this.fsw.EnableRaisingEvents = true;
            this.fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            foreach (string item in fswArgs[1].Split(','))
            {
                if (this.fswSetup.ContainsKey(item))
                {
                    this.fswSetup[item]();
                }
            }
        }

        private FileSystemWatcher SetupFsw()
        {
            if (this.isDirectoryWatcher)
            {
                return new FileSystemWatcher(this.fswArgs[0], this.fswArgs[2]);
            }
            else
            {
                return new FileSystemWatcher(
                    Path.GetDirectoryName(this.fswArgs[0]),
                    Path.GetFileName(this.fswArgs[0]));
            }
        }

        private void SetupSubDirectories()
        {
            if (this.fswArgs.Length == 4 && this.isDirectoryWatcher)
            {
                this.fsw.IncludeSubdirectories = bool.Parse(this.fswArgs[3]);
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

        private void VerifyArguments(string[] args)
        {
            if (string.IsNullOrWhiteSpace(args[0]))
            {
                throw new ArgumentException(Resources.ERROR_EXPECTEDSINGLE, "args[0]");
            }

            if (string.IsNullOrWhiteSpace(Path.GetFileName(args[0])) && args.Length < 3)
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
            if (e.Name == this.fileName)
            {
                this.CreateMessage("Rename", e.FullPath);
            }
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
    }
}