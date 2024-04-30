using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Component.File.Utilities
{
    public interface IFileUtils
    {
        Task CopyFileAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken);

        Task CreateDirectoryAsync(string directory, CancellationToken cancellationToken);

        Task DeleteAsync(string file, CancellationToken cancellationToken);

        Task MoveAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken);

        Task WriteAllBytesAsync(string path, byte[] file, bool overwrite, CancellationToken cancellationToken);

        Task WriteAllTextAsync(string path, string file, bool overwrite, CancellationToken cancellationToken);
    }
}