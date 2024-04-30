using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Enums;
using Kyameru.Component.Ftp.Extensions;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Component.Ftp
{
    /// <summary>
    /// Ftp Client.
    /// </summary>
    internal class FtpClient
    {
        /// <summary>
        /// Ftp settings.
        /// </summary>
        private readonly FtpSettings settings;

        /// <summary>
        /// Web request utility.
        /// </summary>
        private readonly IWebRequestUtility webRequestUtility;

        /// <summary>
        /// Temporary directory to store files.
        /// </summary>
        private const string TMPDIR = "ftp_temp";

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpClient"/> class.
        /// </summary>
        /// <param name="ftpSettings">Ftp settings.</param>
        /// <param name="webRequestUtility">Web request facility.</param>
        public FtpClient(FtpSettings ftpSettings, IWebRequestUtility webRequestUtility)
        {
            this.settings = ftpSettings;
            this.webRequestUtility = webRequestUtility;
        }

        /// <summary>
        /// Event raised for logging.
        /// </summary>
        public event EventHandler<string> OnLog;

        /// <summary>
        /// Event raised when erroring.
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Event raised when downloading a file.
        /// </summary>
        public event EventHandler<Kyameru.Core.Entities.Routable> OnDownloadFile;

        /// <summary>
        /// Poll the endpoint.
        /// </summary>
        internal void Poll()
        {
            List<string> files = new List<string>();
            var getContents = Task<List<string>>.Run(async () =>
            {
                return await this.GetDirectoryContents(default);

            });
            files = getContents.Result;
            if (files?.Count > 0)
            {
                var task = Task.Run(async () =>
                {
                    await DownloadFiles(files, default);
                    await DeleteFiles(files, default);
                });
                task.Wait();
            }
        }

        /// <summary>
        /// Uploads a file to the endpoint.
        /// </summary>
        /// <param name="fileSource">Full source of the file.</param>
        internal async Task UploadFile(string fileSource, CancellationToken cancellationToken)
        {
            await this.UploadFile(System.IO.File.ReadAllBytes(fileSource), System.IO.Path.GetFileName(fileSource), cancellationToken);
        }

        /// <summary>
        /// Uploads a file to the endpoint.
        /// </summary>
        /// <param name="file">Byte array of the file.</param>
        /// <param name="name">Name of the file.</param>
        internal async Task UploadFile(byte[] file, string name, CancellationToken cancellationToken)
        {
            try
            {
                this.RaiseLog(string.Format(Resources.INFO_UPLOADING, name));
                await this.webRequestUtility.UploadFile(file, this.settings, name, cancellationToken);
            }
            catch (Exception ex)
            {
                this.RaiseError(ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes files from the endpoint.
        /// </summary>
        /// <param name="files">List of files.</param>
        private async Task DeleteFiles(List<string> files, CancellationToken cancellationToken)
        {
            if (this.settings.Delete)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    bool closeConnection = i == files.Count - 1;
                    try
                    {
                        this.RaiseLog(string.Format(Resources.INFO_DELETINGFILE, files[i]));
                        await this.webRequestUtility.DeleteFile(settings, files[i], closeConnection, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        this.RaiseError(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Downloads files from the endpoint.
        /// </summary>
        /// <param name="files"></param>
        private async Task DownloadFiles(List<string> files, CancellationToken cancellationToken)
        {
            for (int i = 0; i < files.Count; i++)
            {
                try
                {
                    string transfer = $"{TMPDIR}/{files[i]}";
                    if (!Directory.Exists(TMPDIR))
                    {
                        Directory.CreateDirectory(TMPDIR);
                    }
                    byte[] file = await this.webRequestUtility.DownloadFile(files[i], this.settings, cancellationToken);
                    this.CreateAndRoute(transfer, file);
                }
                catch (Exception ex)
                {
                    this.RaiseError(ex);
                }
            }
        }

        /// <summary>
        /// Create a routable message.
        /// </summary>
        /// <param name="sourceFile">Full source of the file.</param>
        /// <param name="file">Byte array of the file.</param>
        private void CreateAndRoute(string sourceFile, byte[] file)
        {
            FileInfo info = new FileInfo(sourceFile);
            sourceFile = sourceFile.Replace("\\", "/");
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("SourceDirectory", System.IO.Path.GetDirectoryName(sourceFile));
            headers.Add("SourceFile", System.IO.Path.GetFileName(sourceFile));
            headers.Add("FullSource", sourceFile);
            headers.Add("DateCreated", info.CreationTimeUtc.ToLongDateString());
            headers.Add("Readonly", info.IsReadOnly.ToString());
            headers.Add("DataType", "byte");
            headers.Add("FtpSource", this.ConstructFtpUri(this.settings.Path, System.IO.Path.GetFileName(sourceFile)));
            Routable dataItem = new Routable(headers, file);
            this.RaiseOnDownload(dataItem);
        }

        /// <summary>
        /// Gets the directory list of the endpoint.
        /// </summary>
        /// <returns>Returns a list of files and folders.</returns>
        private async Task<List<string>> GetDirectoryContents(CancellationToken cancellationToken)
        {
            List<string> response = null;
            try
            {
                response = await this.webRequestUtility.GetDirectoryContents(this.settings, cancellationToken);
            }
            catch (Exception ex)
            {
                this.RaiseError(ex);
            }

            return response;
        }

        /// <summary>
        /// Construct a valid ftp uri.
        /// </summary>
        /// <param name="path">Path on server.</param>
        /// <param name="file">File name.</param>
        /// <returns></returns>
        private string ConstructFtpUri(string path, string file)
        {
            StringBuilder response = new StringBuilder($"ftp://{this.settings.Host}:{this.settings.Port}/");
            if (!path.IsNullOrEmptyPath())
            {
                response.Append($"{path}/");
            }

            if (!string.IsNullOrWhiteSpace(file))
            {
                response.Append($"{file}");
            }

            return response.ToString();
        }

        /// <summary>
        /// Raises the log event.
        /// </summary>
        /// <param name="message">Log message.</param>
        private void RaiseLog(string message)
        {
            this.OnLog?.Invoke(this, message);
        }

        /// <summary>
        /// Raises the error event.
        /// </summary>
        /// <param name="ex">Exception.</param>
        private void RaiseError(Exception ex)
        {
            this.OnError?.Invoke(this, ex);
        }

        /// <summary>
        /// Raises the on download event.
        /// </summary>
        /// <param name="routable">Message to route.</param>
        private void RaiseOnDownload(Routable routable)
        {
            this.OnDownloadFile?.Invoke(this, routable);
        }
    }
}