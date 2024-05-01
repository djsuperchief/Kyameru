using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Kyameru.Component.File.Tests")]

namespace Kyameru.Component.File.Utilities
{
    internal class FileUtils : IFileUtils
    {
        public async Task WriteAllBytesAsync(string path, byte[] file, bool overwrite, CancellationToken cancellationToken)
        {
            if (overwrite)
            {
                this.OverwriteDestination(path);
            }

            await System.IO.File.WriteAllBytesAsync(path, file, cancellationToken);
        }

        public async Task WriteAllTextAsync(string path, string file, bool overwrite, CancellationToken cancellationToken)
        {
            if (overwrite)
            {
                this.OverwriteDestination(path);
            }

            await System.IO.File.WriteAllTextAsync(path, file, cancellationToken);
        }

        public async Task MoveAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                if (overwrite)
                {
                    this.OverwriteDestination(destination);
                }

                System.IO.File.Move(source, destination);
            }


            await Task.CompletedTask;
        }

        public async Task CopyFileAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                if (overwrite)
                {
                    this.OverwriteDestination(destination);
                }

                System.IO.File.Copy(source, destination);
            }

            await Task.CompletedTask;
        }

        public async Task CreateDirectoryAsync(string directory, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Directory.CreateDirectory(directory);
            }

            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string file, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                System.IO.File.Delete(file);
            }

            await Task.CompletedTask;
        }

        private void OverwriteDestination(string file)
        {
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }
    }
}