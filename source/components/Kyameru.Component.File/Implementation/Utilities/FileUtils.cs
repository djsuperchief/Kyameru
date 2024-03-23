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
        public void WriteAllBytes(string path, byte[] file, bool overwrite = false)
        {
            if (overwrite)
            {
                this.OverwriteDestination(path);
            }

            System.IO.File.WriteAllBytes(path, file);
        }
        
        public async Task WriteAllBytesAsync(string path, byte[] file, bool overwrite, CancellationToken cancellationToken)
        {
            if (overwrite)
            {
                this.OverwriteDestination(path);
            }

            await System.IO.File.WriteAllBytesAsync(path, file, cancellationToken);
        }

        public void WriteAllText(string path, string file, bool overwrite)
        {
            if(overwrite)
            {
                this.OverwriteDestination(path);
            }

            System.IO.File.WriteAllText(path, file);
        }
        
        public async Task WriteAllTextAsync(string path, string file, bool overwrite, CancellationToken cancellationToken)
        {
            if(overwrite)
            {
                this.OverwriteDestination(path);
            }

            await System.IO.File.WriteAllTextAsync(path, file, cancellationToken);
        }

        public void Move(string source, string destination, bool overwrite = false)
        {
            if(overwrite)
            {
                this.OverwriteDestination(destination);
            }

            System.IO.File.Move(source, destination);
        }

        public Task MoveAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => System.IO.File.Move(source, destination), cancellationToken);
        }

        public Task CopyFileAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (overwrite)
                {
                    this.OverwriteDestination(destination);
                }

                System.IO.File.Copy(source, destination);
            }, cancellationToken);
        }

        public void CreateDirectory(string directory) => System.IO.Directory.CreateDirectory(directory);
        public Task CreateDirectoryAsync(string directory, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => System.IO.Directory.CreateDirectory(directory), cancellationToken);
        }

        public void CopyFile(string source, string destination, bool overwrite = false)
        {
            if(overwrite)
            {
                this.OverwriteDestination(destination);
            }

            System.IO.File.Copy(source, destination);
        }

        public void Delete(string file) => System.IO.File.Delete(file);

        public Task DeleteAsync(string file, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => System.IO.File.Delete(file), cancellationToken);
        }

        private void OverwriteDestination(string file)
        {
            if(System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }
    }
}