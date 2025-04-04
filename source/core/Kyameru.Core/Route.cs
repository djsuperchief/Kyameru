﻿using System;
using System.Collections.Generic;
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
            return new RouteBuilder(componentUri, Assembly.GetCallingAssembly());
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
            // Todo: Add processing here for doing the first to route for when condition
            Builder final = builder.ToBuilder();

            if (config.To.Length >= 1)
            {
                for (var i = 0; i < config.To.Length; i++)
                {
                    switch (config.To[i].RegistrationType)
                    {
                        case Core.Enums.ConfigToRegistrationType.To:
                            final.To(config.To[i].ToString());
                            break;
                        case Core.Enums.ConfigToRegistrationType.ToWithPost:
                            final.To(config.To[i].ToString(), config.To[i].PostProcess);
                            break;
                        case Core.Enums.ConfigToRegistrationType.When:
                            final.When(config.To[i].When, config.To[i].ToString());
                            break;
                        case Core.Enums.ConfigToRegistrationType.WhenWithPost:
                            final.When(config.To[i].When, config.To[i].ToString(), config.To[i].PostProcess);
                            break;
                    }
                }
            }

            if (config.Options is { RaiseExceptions: true })
            {
                final.RaiseExceptions();
            }

            if (config.Options?.ScheduleEvery != null)
            {
                final.ScheduleEvery(config.Options.ScheduleEvery.TimeUnit, config.Options.ScheduleEvery.Value);
            }

            if (config.Options?.ScheduleAt != null)
            {
                final.ScheduleEvery(config.Options.ScheduleAt.TimeUnit, config.Options.ScheduleAt.Value);
            }

            return final;
        }

    }
}