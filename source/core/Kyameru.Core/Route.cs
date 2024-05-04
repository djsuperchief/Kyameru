using System.Reflection;
using Kyameru.Core;
using Kyameru.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable ClassNeverInstantiated.Global

namespace Kyameru
{
    /// <summary>
    /// Core route builder utility.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Route"/> class from being created.
        /// </summary>
        private Route()
        {
        }

        /// <summary>
        /// Creates an instance of the route builder.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public static RouteBuilder From(string componentUri)
        {
            return new RouteBuilder(componentUri);
        }

        /// <summary>
        /// Runs a Kyameru build from a config file.
        /// </summary>
        /// <param name="config">Deserialized config file.</param>
        /// <param name="services">Service collection.</param>
        public static void FromConfig(RouteConfig config, IServiceCollection services)
        {
            var final = ConfigBuilder(config, Assembly.GetCallingAssembly());
            final.Build(services);
        }

        private static Builder ConfigBuilder(RouteConfig config, Assembly callingAssembly)
        {
            var builder = new RouteBuilder(config.From.ToString(), callingAssembly);
            foreach (var processor in config.Process)
            {
                builder.Process(processor);
            }

            // This is not great (understatement). Need to refactor this.
            Builder final = null;
            if (!string.IsNullOrWhiteSpace(config.To[0].PostProcess))
            {
                final = builder.To(config.To[0].ToString(), config.To[0].PostProcess);
            }
            else
            {
                final = builder.To(config.To[0].ToString());
            }

            if (config.To.Length > 1)
            {
                for (var i = 1; i < config.To.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(config.To[i].PostProcess))
                    {
                        final.To(config.To[i].ToString(), config.To[i].PostProcess);
                    }
                    else
                    {
                        final.To(config.To[i].ToString());
                    }
                }
            }

            if (config.Options is { RaiseExceptions: true })
            {
                final.RaiseExceptions();
            }
            return final;
        }
        
        
    }
}