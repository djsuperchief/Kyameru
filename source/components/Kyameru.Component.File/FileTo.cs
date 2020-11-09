using System;
using System.Collections.Generic;
using System.IO;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.File
{
    public class FileTo : Core.Contracts.IToComponent
    {
        private readonly Dictionary<string, Action<Routable>> toActions = new Dictionary<string, Action<Routable>>();
        private readonly Dictionary<string, string> headers;

        public event EventHandler<Log> OnLog;

        public FileTo(string[] args)
        {
            this.SetupInternalActions();
            this.headers = args.ToToConfig();
        }

        public FileTo(Dictionary<string, string> incomingHeaders)
        {
            this.SetupInternalActions();
            this.headers = incomingHeaders.ToToConfig();
        }

        public void Process(Routable item)
        {
            this.toActions[this.headers["Action"]](item);
        }

        private void SetupInternalActions()
        {
            this.toActions.Add("Move", this.MoveFile);
            this.toActions.Add("Copy", this.CopyFile);
            this.toActions.Add("Delete", this.DeleteFile);
            this.toActions.Add("Write", this.WriteFile);
        }

        private void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            this.OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }

        private void WriteFile(Routable item)
        {
            System.IO.File.WriteAllBytes(this.GetDestination(item.Headers["SourceFile"]), (byte[])item.Data);
            this.DeleteFile(item);
        }

        private void MoveFile(Routable item)
        {
            this.Log(LogLevel.Information, "Moving file");
            try
            {
                System.IO.File.Move(item.Headers["FullSource"], this.GetDestination(item.Headers["SourceFile"]));
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, "Error Moving File", ex);
            }
        }

        private void CopyFile(Routable item)
        {
            System.IO.File.Copy(item.Headers["FullSource"], this.GetDestination(item.Headers["SourceFile"]));
        }

        private void DeleteFile(Routable item)
        {
            System.IO.File.Delete(item.Headers["FullSource"]);
        }

        private string GetDestination(string filename)
        {
            return Path.Combine(this.headers["Destination"], filename);
        }
    }
}