﻿using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Enums;
using Kyameru.Component.Ftp.Extensions;
using Kyameru.Component.Ftp.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Component.Ftp.Components
{
    /// <summary>
	/// Web request facility.
	/// </summary>
    [ExcludeFromCodeCoverage]
    internal class WebRequestUtility : IWebRequestUtility
    {
        /// <summary>
        /// Logging event.
        /// </summary>
        public event EventHandler<string> OnLog;

        /// <summary>
        /// Dictionary for ftp operations.
        /// </summary>
        private readonly Dictionary<FtpOperation, string> ftpClientOperation = new Dictionary<FtpOperation, string>()
        {
            { FtpOperation.List, WebRequestMethods.Ftp.ListDirectory},
            { FtpOperation.Delete, WebRequestMethods.Ftp.DeleteFile },
            { FtpOperation.Download, WebRequestMethods.Ftp.DownloadFile },
            { FtpOperation.Upload, WebRequestMethods.Ftp.UploadFile }
        };

        /// <summary>
        /// Deletes a file from the endpoint.
        /// </summary>
        /// <param name="settings">Ftp Settings.</param>
        /// <param name="fileName">Name of file to updload.</param>
        /// <param name="closeConnection">Value indicating whether the connection sould be closed.</param>
        public async Task DeleteFile(FtpSettings settings, string fileName, bool closeConnection = true, CancellationToken cancellationToken = default)
        {
            this.RaiseLog($"Deleting file {fileName}");
            FtpWebRequest request = this.GetFtpWebRequest($"{settings.Path.StripEndingSlash()}/{fileName}", FtpOperation.Delete, settings, closeConnection);
            if (!cancellationToken.IsCancellationRequested)
            {
                await request.GetResponseAsync();
            }
        }

        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <param name="fileName">Filename of the file to download.</param>
        /// <param name="settings">Ftp settings.</param>
        /// <returns>Returns a byte array of the file.</returns>
        public async Task<byte[]> DownloadFile(string fileName, FtpSettings settings, CancellationToken cancellationToken)
        {
            byte[] file = null;
            this.RaiseLog($"Downloading file {fileName}");
            FtpWebRequest request = this.GetFtpWebRequest($"{settings.Path.StripEndingSlash()}/{fileName}", FtpOperation.Download, settings);
            if (!cancellationToken.IsCancellationRequested)
            {
                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                using (MemoryStream responseStream = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(responseStream);
                    file = responseStream.ToArray();
                }
            }

            return file;
        }

        /// <summary>
        /// Gets the directory listing from the endpoint.
        /// </summary>
        /// <param name="settings">Ftp Settinfs.</param>
        /// <returns>Returns a list of files and directories.</returns>
        public async Task<List<string>> GetDirectoryContents(FtpSettings settings, CancellationToken cancellationToken)
        {
            this.RaiseLog("Getting FTP directory contents...");
            List<string> response = new List<string>();
            FtpWebRequest ftp = this.GetFtpWebRequest(settings.Path, FtpOperation.List, settings, false);
            if (!cancellationToken.IsCancellationRequested)
            {
                using (FtpWebResponse ftpResponse = (FtpWebResponse)await ftp.GetResponseAsync())
                using (Stream responseStream = ftpResponse.GetResponseStream())
                {
                    this.RaiseLog(Resources.INFO_GETTINGDIRECTORY);
                    StreamReader reader = new StreamReader(responseStream);
                    while (!reader.EndOfStream)
                    {
                        string file = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(Path.GetExtension(file)))
                        {
                            response.Add(file);
                        }
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Uploads a file.
        /// </summary>
        /// <param name="file">File byte array.</param>
        /// <param name="settings">Ftp settings.</param>
        /// <param name="fileName">Name of file to upload.</param>
        public async Task UploadFile(byte[] file, FtpSettings settings, string fileName, CancellationToken cancellationToken)
        {
            this.RaiseLog("Uploading file to FTP");
            string path = settings.Path.Substring(settings.Path.Length - 1, 1) == "/" ? settings.Path.Substring(0, settings.Path.Length - 1) : settings.Path;
            FtpWebRequest request = this.GetFtpWebRequest($"{path}/{fileName}", FtpOperation.Upload, settings, true);
            request.ContentLength = file.Length;
            if (!cancellationToken.IsCancellationRequested)
            {
                using (Stream ftpStream = request.GetRequestStream())
                {
                    await ftpStream.WriteAsync(file, 0, file.Length, cancellationToken);
                }
            }
        }

        private FtpWebRequest GetFtpWebRequest(string path, FtpOperation method, Settings.FtpSettings settings, bool closeConnection = false)
        {
            if (path.Substring(0, 1) != "/")
            {
                path = $"/{path}";
            }

            FtpWebRequest response = (FtpWebRequest)WebRequest.Create($"ftp://{settings.Host}:{settings.Port}{path}");
            response.Method = this.ftpClientOperation[method];
            response.UseBinary = true;
            response.KeepAlive = !closeConnection;
            if (settings.Credentials != null)
            {
                response.Credentials = settings.Credentials;
            }

            return response;
        }

        /// <summary>
        /// Raises a log message.
        /// </summary>
        /// <param name="message"></param>
        private void RaiseLog(string message)
        {
            this.OnLog?.Invoke(this, message);
        }
    }
}