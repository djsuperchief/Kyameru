using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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

        public void WriteAllText(string path, string file, bool overwrite)
        {
            if(overwrite)
            {
                this.OverwriteDestination(path);
            }

            System.IO.File.WriteAllText(path, file);
        }

        public void Move(string source, string destination, bool overwrite = false)
        {
            if(overwrite)
            {
                this.OverwriteDestination(destination);
            }

            System.IO.File.Move(source, destination);
        }

        public void CreateDirectory(string directory) => System.IO.Directory.CreateDirectory(directory);

        public void CopyFile(string source, string destination, bool overwrite = false)
        {
            if(overwrite)
            {
                this.OverwriteDestination(destination);
            }

            System.IO.File.Copy(source, destination);
        }

        public void Delete(string file) => System.IO.File.Delete(file);

        private void OverwriteDestination(string file)
        {
            if(System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }
    }
}