using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kyameru.Component.File.Utilities
{
    public interface IFileSystemWatcher : IDisposable
    {
        bool EnableRaisingEvents { get; set; }
        NotifyFilters NotifyFilter { get; set; }
        bool IncludeSubdirectories { get; set; }
        string Path { get; set; }
        string Filter { get; set; }

        event FileSystemEventHandler Deleted;

        event FileSystemEventHandler Created;

        event FileSystemEventHandler Changed;

        event RenamedEventHandler Renamed;
    }
}