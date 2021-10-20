using Kyameru.Component.Ftp.Enums;
using Kyameru.Component.Ftp.Settings;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        void UploadFile(byte[] file, FtpSettings settings, string fileName);

        /// <summary>
        /// Deletes a file on the Ftp endpoint.
        /// </summary>
        /// <param name="settings">Ftp Settings.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="closeConnection">Value indicating whether the session should be closed.</param>
        void DeleteFile(FtpSettings settings, string fileName, bool closeConnection = true);

        /// <summary>
        /// Downloads a file from the Ftp endpoint.
        /// </summary>
        /// <param name="fileName">File to download.</param>
        /// <param name="settings">Ftp settings.</param>
        /// <returns>Returns the byte array of the file.</returns>
        byte[] DownloadFile(string fileName, FtpSettings settings);

        /// <summary>
        /// Gets the Ftp directory list.
        /// </summary>
        /// <param name="settings">Ftp Settings.</param>
        /// <returns>Returns a list of directory contents.</returns>
        List<string> GetDirectoryContents(FtpSettings settings);
    }
}