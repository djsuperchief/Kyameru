using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Component.File.Utilities
{
    public interface IFileUtils
    {
        void CopyFile(string source, string destination, bool overwrite);
        Task CopyFileAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken);

        void CreateDirectory(string directory);
        
        Task CreateDirectoryAsync(string directory, CancellationToken cancellationToken);

        void Delete(string file);

        Task DeleteAsync(string file, CancellationToken cancellationToken);

        void Move(string source, string destination, bool overwrite);

        Task MoveAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken);

        void WriteAllBytes(string path, byte[] file, bool overwrite);
        
        Task WriteAllBytesAsync(string path, byte[] file, bool overwrite, CancellationToken cancellationToken);

        void WriteAllText(string path, string file, bool overwrite);
        
        Task WriteAllTextAsync(string path, string file, bool overwrite, CancellationToken cancellationToken);
    }
}