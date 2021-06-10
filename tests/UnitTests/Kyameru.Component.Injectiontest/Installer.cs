using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.Injectiontest
{
    public static class Installer
    {
        public static IServiceCollection InstallToService(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddTransient<IMyTo, MyTo>();
            
            return serviceDescriptors;
        }

        public static IServiceCollection InstallFromService(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddTransient<IMyFrom, MyFrom>();
            return serviceDescriptors;
        }
    }
}
