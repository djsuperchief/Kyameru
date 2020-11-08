using System;
using System.Collections.Generic;
using System.IO;
using Kyameru.Core.Entities;

namespace Kyameru.Component.File
{
    public class Single : Kyameru.Core.Contracts.IFromComponent, Kyameru.Core.Contracts.IToComponent
    {
        public event EventHandler<Routable> OnAction;
        private string fileName;
        private FileSystemWatcher fsw;
        private string[] fswArgs;

        private readonly Dictionary<string, Action> fswSetup = new Dictionary<string, Action>();
        private readonly Dictionary<string, Action<Routable, string>> toActions = new Dictionary<string, Action<Routable, string>>();

        public Single()
        {
            this.SetupInternalActions();
        }

        public void Process(Routable item)
        {
            throw new Core.Exceptions.ProcessException(Resources.ERROR_MUSTSPECIFYPROCESSARGS);
        }

        public void Process(Routable item, string[] args)
        {
            this.toActions[args[0]](item, args[1]);
        }

        public void Setup(string[] args)
        {
            this.VerifyArguments(args);
            this.fileName = args[0];
            this.fswArgs = args;
        }

        

        public void Start()
        {
            this.fsw = new FileSystemWatcher(
                System.IO.Path.GetDirectoryName(fswArgs[0]),
                System.IO.Path.GetFileName(fswArgs[0]));
            this.fsw.IncludeSubdirectories = false;
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

        public void Stop()
        {
            this.fsw.Dispose();
        }

        public void LogInformation(string info)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string warning)
        {
            throw new NotImplementedException();
        }

        public void LogError(string error)
        {
            throw new NotImplementedException();
        }

        public void LogCritical(string critical)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception ex)
        {
            throw new NotImplementedException();
        }

        

        private void VerifyArguments(string[] args)
        {
            if(string.IsNullOrWhiteSpace(System.IO.Path.GetExtension(args[0])))
            {
                throw new ArgumentException(Resources.ERROR_EXPECTEDSINGLE, "args[0]");
            }
        }

        private void SetupInternalActions()
        {
            this.fswSetup.Add("Created", this.AddCreated);
            this.fswSetup.Add("Changed", this.AddChanged);
            this.fswSetup.Add("Renamed", this.AddRenamed);
            this.toActions.Add("Move", this.MoveFile);
            this.toActions.Add("Copy", this.CopyFile);
            this.toActions.Add("Delete", this.DeleteFile);
            this.toActions.Add("Write", this.WriteFile);
        }

        private void WriteFile(Routable item, string arg)
        {
            System.IO.File.WriteAllBytes($"{arg}{Path.DirectorySeparatorChar}{item.Headers["SourceFile"]}", (byte[])item.Data);
            this.DeleteFile(item, string.Empty);
        }

        private void MoveFile(Routable item, string arg)
        {
            System.IO.File.Move(item.Headers["FullSource"], arg);
        }

        private void CopyFile(Routable item, string arg)
        {
            System.IO.File.Copy(item.Headers["FullSource"], arg);
        }

        private void DeleteFile(Routable item, string arg)
        {
            System.IO.File.Delete(item.Headers["FullSource"]);
        }

        private void AddRenamed()
        {
            this.fsw.Renamed += Fsw_Renamed;
        }

        private void Fsw_Renamed(object sender, RenamedEventArgs e)
        {
            if(e.Name == this.fileName)
            {
                this.CreateMessage("Rename");
            }
        }

        private void AddChanged()
        {
            this.fsw.Changed += Fsw_Changed;
        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            this.CreateMessage("Changed");
        }

        private void AddCreated()
        {
            this.fsw.Created += Fsw_Created;
        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            this.CreateMessage("Created");
        }

        private void CreateMessage(string method)
        {
            FileInfo info = new FileInfo(this.fileName);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("SourceDirectory", System.IO.Path.GetDirectoryName(this.fileName));
            headers.Add("SourceFile", System.IO.Path.GetFileName(this.fileName));
            headers.Add("FullSource", this.fileName);
            headers.Add("DateCreated", info.CreationTimeUtc.ToLongTimeString());
            headers.Add("Readonly", info.IsReadOnly.ToString());
            headers.Add("ReadType", "byte");
            Routable dataItem = new Routable(headers, System.IO.File.ReadAllBytes(this.fileName));
            this.OnAction?.Invoke(this, dataItem);
        }
    }
}
