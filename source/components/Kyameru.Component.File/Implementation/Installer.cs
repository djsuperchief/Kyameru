using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.File
{
    internal static class Installer
    {
        public static IServiceCollection InstallToService(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddTransient<IFileTo, FileTo>();
            return serviceDescriptors;
        }

        public static IServiceCollection InstallFromService(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddTransient<IFileWatcher, FileWatcher>();
            return serviceDescriptors;
        }
    }
}
