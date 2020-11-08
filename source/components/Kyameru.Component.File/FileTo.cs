﻿using System;
using System.Collections.Generic;
using System.IO;
using Kyameru.Core.Entities;

namespace Kyameru.Component.File
{
    public class FileTo : Core.Contracts.IToComponent
    {
        private readonly Dictionary<string, Action<Routable>> toActions = new Dictionary<string, Action<Routable>>();
        private readonly string DestinationFolder;
        private readonly string Action;

        public FileTo(string[] args)
        {
            this.SetupInternalActions();
            this.DestinationFolder = args[0];
            this.Action = args[1];
        }

        public void Process(Routable item)
        {
            this.toActions[Action](item);
        }

        public void LogCritical(string critical)
        {
            throw new NotImplementedException();
        }

        public void LogError(string error)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(string info)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string warning)
        {
            throw new NotImplementedException();
        }

        private void SetupInternalActions()
        {
            this.toActions.Add("Move", this.MoveFile);
            this.toActions.Add("Copy", this.CopyFile);
            this.toActions.Add("Delete", this.DeleteFile);
            this.toActions.Add("Write", this.WriteFile);
        }

        private void WriteFile(Routable item)
        {
            System.IO.File.WriteAllBytes(this.GetDestination(item.Headers["SourceFile"]), (byte[])item.Data);
            this.DeleteFile(item);
        }

        private void MoveFile(Routable item)
        {
            System.IO.File.Move(item.Headers["FullSource"], this.GetDestination(item.Headers["SourceFile"]));
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
            return Path.Combine(this.DestinationFolder, filename);
        }
    }
}