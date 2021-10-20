using System;
using System.Collections.Generic;
using System.IO;
using Kyameru.Component.File.Utilities;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.File
{
    /// <summary>
    /// To Component.
    /// </summary>
    public class FileTo : IFileTo
    {
        /// <summary>
        /// Valid actions
        /// </summary>
        private readonly Dictionary<string, Action<Routable>> toActions = new Dictionary<string, Action<Routable>>();

        /// <summary>
        /// Valid headers
        /// </summary>
        private readonly Dictionary<string, string> headers;

        /// <summary>
        /// File utilities.
        /// </summary>
        /// <remarks>
        /// To allow testing of all code paths.
        /// </remarks>
        private readonly IFileUtils fileUtils;

        /// <summary>
        /// Value indicating whether the overwrite facility should be used.
        /// </summary>
        private readonly bool overwrite;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTo"/> class.
        /// </summary>
        /// <param name="incomingHeaders">Incoming headers.</param>
        public FileTo(Dictionary<string, string> incomingHeaders, IFileUtils fileUtils)
        {
            this.SetupInternalActions();
            this.headers = incomingHeaders.ToToConfig();
            this.fileUtils = fileUtils;
            this.overwrite = bool.Parse(this.headers["Overwrite"]);
        }

        /// <summary>
        /// Log event.
        /// </summary>
        public event EventHandler<Log> OnLog;

        /// <summary>
        /// Process the message.
        /// </summary>
        /// <param name="item">Message to process.</param>
        public void Process(Routable item)
        {
            this.toActions[this.headers["Action"]](item);
        }

        /// <summary>
        /// Sets up internal delegates.
        /// </summary>
        private void SetupInternalActions()
        {
            this.toActions.Add("Move", this.MoveFile);
            this.toActions.Add("Copy", this.CopyFile);
            this.toActions.Add("Delete", this.DeleteFile);
            this.toActions.Add("Write", this.WriteFile);
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
        /// Writes a file to disk.
        /// </summary>
        /// <param name="item">Message to process.</param>
        private void WriteFile(Routable item)
        {
            this.Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_WRITE, item.Headers["SourceFile"]));
            try
            {
                this.EnsureDestinationExists();
                if(item.Headers["DataType"] == "String")
                {
                    this.fileUtils.WriteAllText(this.GetDestination(item.Headers["SourceFile"]), (string)item.Body, this.overwrite);
                }
                else
                {
                    this.fileUtils.WriteAllBytes(this.GetDestination(item.Headers["SourceFile"]), (byte[])item.Body, this.overwrite);
                }
                
                this.DeleteFile(item);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, Resources.ERROR_ACTION_WRITE, ex);
                item.SetInError(this.RaiseError("WriteFile", "Error writing file"));
            }
        }

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="item">Message to process.</param>
        private void MoveFile(Routable item)
        {
            this.Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_MOVE, item.Headers["SourceFile"]));
            try
            {
                this.EnsureDestinationExists();
                this.fileUtils.Move(item.Headers["FullSource"], this.GetDestination(item.Headers["SourceFile"]), this.overwrite);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, Resources.ERROR_ACTION_MOVE, ex);
                item.SetInError(this.RaiseError("MoveFile", "Error writing file"));
            }
        }

        /// <summary>
        /// Ensures destination folder exists.
        /// </summary>
        private void EnsureDestinationExists()
        {
            if (!System.IO.Directory.Exists(this.headers["Target"]))
            {
                this.fileUtils.CreateDirectory(this.headers["Target"]);
            }
        }

        /// <summary>
        /// Copies the source file but leaves original in place.
        /// </summary>
        /// <param name="item">Message to process.</param>
        private void CopyFile(Routable item)
        {
            this.Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_COPY, item.Headers["SourceFile"]));
            try
            {
                this.EnsureDestinationExists();
                this.fileUtils.CopyFile(item.Headers["FullSource"], this.GetDestination(item.Headers["SourceFile"]), this.overwrite);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, Resources.ERROR_ACTION_COPY, ex);
                item.SetInError(this.RaiseError("CopyFile", "Error writing file"));
            }
        }

        /// <summary>
        /// Deletes source file.
        /// </summary>
        /// <param name="item">Message to process.</param>
        private void DeleteFile(Routable item)
        {
            this.Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_DELETE, item.Headers["SourceFile"]));
            try
            {
                this.fileUtils.Delete(item.Headers["FullSource"]);
            }
            catch (Exception ex)
            {
                this.Log(LogLevel.Error, Resources.ERROR_ACTION_DELETE, ex);
                item.SetInError(this.RaiseError("DeleteError", "Error writing file"));
            }
        }

        /// <summary>
        /// Constructs the destination.
        /// </summary>
        /// <param name="filename">Source file.</param>
        /// <returns>Returns a valid destination for the file.</returns>
        private string GetDestination(string filename)
        {
            return Path.Combine(this.headers["Target"], filename);
        }

        /// <summary>
        /// Raises an error object.
        /// </summary>
        /// <param name="action">Current action.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Returns a new instance of the <see cref="Error"/> object.</returns>
        private Error RaiseError(string action, string message)
        {
            return new Error("ToFile", action, message);
        }
    }
}