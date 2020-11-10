﻿using System;
using System.Collections.Generic;
using System.Data;
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
            this.Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_WRITE, item.Headers["SourceFile"]));
            try
            {
                System.IO.File.WriteAllBytes(this.GetDestination(item.Headers["SourceFile"]), (byte[])item.Body);
                this.DeleteFile(item);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, Resources.ERROR_ACTION_WRITE, ex);
            }
        }

        private void MoveFile(Routable item)
        {
            this.Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_MOVE, item.Headers["SourceFile"]));
            try
            {
                System.IO.File.Move(item.Headers["FullSource"], this.GetDestination(item.Headers["SourceFile"]));
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, Resources.ERROR_ACTION_MOVE, ex);
            }
        }

        private void CopyFile(Routable item)
        {
            this.Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_COPY, item.Headers["SourceFile"]));
            try
            {
                System.IO.File.Copy(item.Headers["FullSource"], this.GetDestination(item.Headers["SourceFile"]));
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, Resources.ERROR_ACTION_COPY, ex);
            }
        }

        private void DeleteFile(Routable item)
        {
            this.Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_DELETE, item.Headers["SourceFile"]));
            try
            {
                System.IO.File.Delete(item.Headers["FullSource"]);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, Resources.ERROR_ACTION_DELETE, ex);
            }
        }

        private string GetDestination(string filename)
        {
            return Path.Combine(this.headers["Target"], filename);
        }

        public void SetError(Routable routable)
        {
            routable.SetInError("ToFile");
        }
    }
}