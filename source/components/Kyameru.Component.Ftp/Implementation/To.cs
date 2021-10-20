using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kyameru.Component.Ftp
{
    /// <summary>
    /// To component.
    /// </summary>
    public class To : IToComponent
    {
        /// <summary>
        /// Ftp settings.
        /// </summary>
        private readonly FtpSettings ftpSettings;

        /// <summary>
        /// Ftp client.
        /// </summary>
        private readonly FtpClient ftpClient;

        /// <summary>
        /// Archive path.
        /// </summary>
        private readonly string archivePath;

        /// <summary>
        /// Source path.
        /// </summary>
        private readonly string source;

        /// <summary>
        /// Initializes a new instance of the <see cref="To"/> class.
        /// </summary>
        /// <param name="headers">Valid headers.</param>
        /// <param name="webRequestUtility">Web request utility.</param>
        public To(Dictionary<string, string> headers, IWebRequestUtility webRequestUtility)
        {
            Dictionary<string, string> config = headers.ToToConfig();
            this.archivePath = config.GetKeyValue("Archive");
            this.source = config.GetKeyValue("Source");
            this.ftpSettings = new FtpSettings(config);
            this.ftpClient = new FtpClient(this.ftpSettings, webRequestUtility);
        }

        /// <summary>
        /// Logging event.
        /// </summary>
        public event EventHandler<Log> OnLog;

        /// <summary>
        /// Process Message.
        /// </summary>
        /// <param name="item">Message to process.</param>
        public void Process(Routable item)
        {
            try
            {
                this.ftpClient.UploadFile(this.GetSource(item), item.Headers.GetKeyValue("SourceFile"));
                this.ArchiveFile(item);
            }
            catch (Exception ex)
            {
                item.SetInError(this.GetError("Upload", string.Format(Resources.ERROR_UPLOADING, item.Headers.GetKeyValue("FileName"))));
                this.RaiseLog(string.Format(Resources.ERROR_UPLOADING, item.Headers.GetKeyValue("FileName")), LogLevel.Error, ex);
            }
        }

        /// <summary>
        /// Archives the outgoing file.
        /// </summary>
        /// <param name="item">Message to process.</param>
        private void ArchiveFile(Routable item)
        {
            if ((this.source == "File" || string.IsNullOrWhiteSpace(this.source)) && !string.IsNullOrWhiteSpace(this.archivePath))
            {
                string fileName = item.Headers["SourceFile"];
                string currentDirectory = System.IO.Directory.GetParent(item.Headers["FullSource"]).FullName;
                string archiveDir = this.GetPath(currentDirectory);
                this.EnsureDirectoryExists(archiveDir);
                System.IO.File.Move(item.Headers["FullSource"], Path.Combine(archiveDir, fileName));
            }
        }

        /// <summary>
        /// Gets the fully qualified path for archiving.
        /// </summary>
        /// <param name="originalPath">Original path for outgoing file.</param>
        /// <returns>Returns the fully qualified path for archive.</returns>
        private string GetPath(string originalPath)
        {
            string response = string.Empty;
            if (this.archivePath.Contains(".."))
            {
                response = new Uri(System.IO.Path.Combine(originalPath, this.archivePath)).LocalPath;
            }
            else
            {
                response = this.archivePath;
            }

            return response;
        }

        /// <summary>
        /// Ensures a directory exists.
        /// </summary>
        /// <param name="path">Path to check.</param>
        private void EnsureDirectoryExists(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(System.IO.Directory.GetParent(path).FullName);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }

        /// <summary>
        /// Gets the source to upload.
        /// </summary>
        /// <param name="item">Message to process.</param>
        /// <returns>Returns a byte array of the file.</returns>
        private byte[] GetSource(Routable item)
        {
            byte[] response = null;
            if (this.source == "File" || string.IsNullOrWhiteSpace(this.source))
            {
                response = System.IO.File.ReadAllBytes(item.Headers.GetKeyValue("FullSource"));
            }
            else
            {
                response = (byte[])item.Body;
            }

            return response;
        }

        /// <summary>
        /// Gets a log entity for logging.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="logLevel">Level of logging.</param>
        /// <param name="ex">Exception to log.</param>
        /// <returns>Returns an instance of the <see cref="Log"/> class.</returns>
        private Log RaiseLog(string message, LogLevel logLevel, Exception ex = null)
        {
            return new Log(logLevel, message, ex);
        }

        /// <summary>
        /// Gets an error entity for tracing.
        /// </summary>
        /// <param name="action">Current action.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Returns an instance of the <see cref="Error"/> class.</returns>
        private Error GetError(string action, string message)
        {
            return new Error("ToFtp", action, message);
        }
    }
}