using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Enums;
using Kyameru.Component.Ftp.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;

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
        public void DeleteFile(FtpSettings settings, string fileName, bool closeConnection = true)
        {
            FtpWebRequest request = this.GetFtpWebRequest($"{settings.Path}/{fileName}", FtpOperation.Delete, settings, closeConnection);
            request.GetResponse();
        }

        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <param name="fileName">Filename of the file to download.</param>
        /// <param name="settings">Ftp settings.</param>
        /// <returns>Returns a byte array of the file.</returns>
        public byte[] DownloadFile(string fileName, FtpSettings settings)
        {
            byte[] file = null;
            FtpWebRequest request = this.GetFtpWebRequest($"{settings.Path}/{fileName}", FtpOperation.Download, settings);
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            using (MemoryStream responseStream = new MemoryStream())
            {
                response.GetResponseStream().CopyTo(responseStream);
                file = responseStream.ToArray();
            }

            return file;
        }

        /// <summary>
        /// Gets the directory listing from the endpoint.
        /// </summary>
        /// <param name="settings">Ftp Settinfs.</param>
        /// <returns>Returns a list of files and directories.</returns>
        public List<string> GetDirectoryContents(FtpSettings settings)
        {
            List<string> response = new List<string>();
            FtpWebRequest ftp = this.GetFtpWebRequest(settings.Path, FtpOperation.List, settings, false);
            using (FtpWebResponse ftpResponse = (FtpWebResponse)ftp.GetResponse())
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

            return response;
        }

        /// <summary>
        /// Uploads a file.
        /// </summary>
        /// <param name="file">File byte array.</param>
        /// <param name="settings">Ftp settings.</param>
        /// <param name="fileName">Name of file to upload.</param>
        public void UploadFile(byte[] file, FtpSettings settings, string fileName)
        {
            string path = settings.Path.Substring(settings.Path.Length - 1, 1) == "/" ? settings.Path.Substring(0, settings.Path.Length - 1) : settings.Path;
            FtpWebRequest request = this.GetFtpWebRequest($"{path}/{fileName}", FtpOperation.Upload, settings, true);
            request.ContentLength = file.Length;
            using (Stream ftpStream = request.GetRequestStream())
            {
                ftpStream.Write(file, 0, file.Length);
            }
        }

        private FtpWebRequest GetFtpWebRequest(string path, FtpOperation method, Settings.FtpSettings settings, bool closeConnection = false)
        {
            if(path.Substring(0,1) != "/")
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