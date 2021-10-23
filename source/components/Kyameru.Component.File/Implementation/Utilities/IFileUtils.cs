namespace Kyameru.Component.File.Utilities
{
    public interface IFileUtils
    {
        void CopyFile(string source, string destination, bool overwrite);

        void CreateDirectory(string directory);

        void Delete(string file);

        void Move(string source, string destination, bool overwrite);

        void WriteAllBytes(string path, byte[] file, bool overwrite);

        void WriteAllText(string path, string file, bool overwrite);
    }
}