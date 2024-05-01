using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Dictionary<string, Func<Routable, CancellationToken, Task>> toActions =
            new Dictionary<string, Func<Routable, CancellationToken, Task>>();

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
            SetupInternalActions();
            headers = incomingHeaders.ToToConfig();
            this.fileUtils = fileUtils;
            overwrite = bool.Parse(headers["Overwrite"]);
        }

        /// <summary>
        /// Log event.
        /// </summary>
        public event EventHandler<Log> OnLog;

        public async Task ProcessAsync(Routable item, CancellationToken cancellationToken)
        {
            await toActions[headers["Action"]](item, cancellationToken);

        }

        /// <summary>
        /// Sets up internal delegates.
        /// </summary>
        private void SetupInternalActions()
        {

            toActions.Add("Move", MoveFileAsync);
            toActions.Add("Copy", this.CopyFileAsync);
            toActions.Add("Delete", DeleteFileAsync);
            toActions.Add("Write", WriteFileAsync);
        }

        /// <summary>
        /// Abstraction for logging event.
        /// </summary>
        /// <param name="logLevel">Log level to raise.</param>
        /// <param name="message">Log message.</param>
        /// <param name="exception">Log exception.</param>
        private void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            OnLog?.Invoke(this, new Log(logLevel, message, exception));
        }


        private async Task WriteFileAsync(Routable item, CancellationToken cancellationToken)
        {
            Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_WRITE, item.Headers["SourceFile"]));
            try
            {
                await EnsureDestinationExistsAsync(cancellationToken);
                if (item.Headers.TryGetValue("DataType", "String") == "String")
                {
                    await fileUtils.WriteAllTextAsync(GetDestination(item.Headers["SourceFile"]),
                        (string)item.Body, overwrite, cancellationToken);
                }
                else
                {
                    await fileUtils.WriteAllBytesAsync(GetDestination(item.Headers["SourceFile"]),
                        (byte[])item.Body, overwrite, cancellationToken);
                }

                await DeleteFileAsync(item, cancellationToken);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, Resources.ERROR_ACTION_WRITE, ex);
                item.SetInError(RaiseError("WriteFile", "Error writing file"));
            }
        }

        private async Task MoveFileAsync(Routable item, CancellationToken cancellationToken)
        {
            Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_MOVE, item.Headers["SourceFile"]));
            try
            {
                await EnsureDestinationExistsAsync(cancellationToken);
                await fileUtils.MoveAsync(item.Headers["FullSource"], GetDestination(item.Headers["SourceFile"]), overwrite, cancellationToken);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, Resources.ERROR_ACTION_MOVE, ex);
                item.SetInError(RaiseError("MoveFile", "Error writing file"));
            }
        }

        private async Task EnsureDestinationExistsAsync(CancellationToken cancellationToken)
        {
            if (!Directory.Exists(headers["Target"]))
            {
                await fileUtils.CreateDirectoryAsync(headers["Target"], cancellationToken);
            }
        }

        private async Task CopyFileAsync(Routable item, CancellationToken cancellationToken)
        {
            Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_COPY, item.Headers["SourceFile"]));
            try
            {
                await EnsureDestinationExistsAsync(cancellationToken);
                await fileUtils.CopyFileAsync(item.Headers["FullSource"], GetDestination(item.Headers["SourceFile"]),
                    overwrite, cancellationToken);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, Resources.ERROR_ACTION_COPY, ex);
                item.SetInError(RaiseError("CopyFile", "Error writing file"));
            }
        }

        private async Task DeleteFileAsync(Routable item, CancellationToken cancellationToken)
        {
            Log(LogLevel.Information, string.Format(Resources.INFO_ACTION_DELETE, item.Headers["SourceFile"]));
            try
            {
                await fileUtils.DeleteAsync(item.Headers["FullSource"], cancellationToken);
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, Resources.ERROR_ACTION_DELETE, ex);
                item.SetInError(RaiseError("DeleteError", "Error writing file"));
            }
        }

        /// <summary>
        /// Constructs the destination.
        /// </summary>
        /// <param name="filename">Source file.</param>
        /// <returns>Returns a valid destination for the file.</returns>
        private string GetDestination(string filename)
        {
            return Path.Combine(headers["Target"], filename);
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