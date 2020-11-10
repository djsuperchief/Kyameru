using System;

namespace Kyameru.Component.File
{
    internal static class Resources
    {
        internal const string ERROR_EXPECTEDSINGLE = "Expected single file input.";
        internal const string ERROR_MUSTSPECIFYPROCESSARGS = "Must specify processing arguments.";
        internal const string ERROR_NOTENOUGHARGUMENTS_DIRECTORY = "Not enough arguments for directory watcher.";
        internal const string INFO_ACTION_MOVE = "Moving File {0}";
        internal const string ERROR_ACTION_MOVE = "Error moving file.";

        internal const string INFO_ACTION_WRITE = "Writing File {0}";
        internal const string ERROR_ACTION_WRITE = "Error writing file.";

        internal const string INFO_ACTION_COPY = "Copying File {0}";
        internal const string ERROR_ACTION_COPY = "Error copying file.";

        internal const string INFO_ACTION_DELETE = "Deleting File {0}";
        internal const string ERROR_ACTION_DELETE = "Error deleting file.";
    }
}