using Kyameru.Component.Ftp.Enums;
using Kyameru.Component.Ftp.Settings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Kyameru.Component.Ftp.Tests")]

namespace Kyameru.Component.Ftp.Contracts
{
    /// <summary>
    /// Web request facility.
    /// </summary>
    /// <remarks>
    /// Primarily for unit tests to enable mocking.
    /// There is the possibility of creating an ftp server as part of the test
    /// but need to get this into beta asap.
    /// </remarks>
    public interface IWebRequestUtility
    {
        /// <summary>
        /// Uploads a file to the FTP endpoint.
        /// </summary>
        /// <param name="file">File Binary.</param>
        /// <param name="settings">Ftp Settings.</param>
        /// <param name="fileName">File Name.</param>
        Task UploadFile(byte[] file, FtpSettings settings, string fileName, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a file on the Ftp endpoint.
        /// </summary>
        /// <param name="settings">Ftp Settings.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="closeConnection">Value indicating whether the session should be closed.</param>
        Task DeleteFile(FtpSettings settings, string fileName, bool closeConnection = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Downloads a file from the Ftp endpoint.
        /// </summary>
        /// <param name="fileName">File to download.</param>
        /// <param name="settings">Ftp settings.</param>
        /// <returns>Returns the byte array of the file.</returns>
        Task<byte[]> DownloadFile(string fileName, FtpSettings settings, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the Ftp directory list.
        /// </summary>
        /// <param name="settings">Ftp Settings.</param>
        /// <returns>Returns a list of directory contents.</returns>
        Task<List<string>> GetDirectoryContents(FtpSettings settings, CancellationToken cancellationToken);

        /// <summary>
        /// Logging event.
        /// </summary>
        event EventHandler<string> OnLog;
    }
}