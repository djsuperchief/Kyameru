using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Component activation interface.
    /// </summary>
    public interface IOasis
    {
        /// <summary>
        /// Creates a from component.
        /// </summary>
        /// <param name="headers">Dictionary of headers to apply.</param>
        /// <param name="isAtomic">Indicates if the route is atomic.</param>
        /// <param name="serviceProvider">DI Service provider</param>
        /// <returns>Returns an instance of the <see cref="IFromChainLink"/> interface.</returns>
        IFromChainLink CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider);

        /// <summary>
        /// Creates a to component.
        /// </summary>
        /// <param name="headers">Dictionary of headers to apply.</param>
        /// <param name="serviceProvider">DI Service provider</param>
        /// <returns>Returns an instance of the <see cref="IToChainLink"/> interface.</returns>
        IToChainLink CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider);

        /// <summary>
        /// Creates an atomic component.
        /// </summary>
        /// <param name="headers">Dictionary of headers to apply.</param>
        /// <returns>Returns an instance of the <see cref="IAtomicLink"/> interface.</returns>
        IAtomicLink CreateAtomicComponent(Dictionary<string, string> headers);

        /// <summary>
        /// Creates a scheduled component.
        /// </summary>
        /// <param name="headers">Dictionary of headers to apply.</param>
        /// <param name="isAtomic">Indicates if the route is atomic.</param>
        /// <param name="serviceProvider">DI Service provider</param>
        /// <returns>Returns an instance of the <see cref="IScheduleChainLink"/> interface.</returns>
        IScheduleChainLink CreateScheduleComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider);

        /// <summary>
        /// Registers internal to services
        /// </summary>
        /// <param name="serviceCollection">IoC collection</param>
        /// <returns>Returns the <see cref="IServiceCollection"/>.</returns>
        IServiceCollection RegisterTo(IServiceCollection serviceCollection);

        /// <summary>
        /// Registers internal from services
        /// </summary>
        /// <param name="serviceCollection">IoC collection</param>
        /// <returns>Returns the <see cref="IServiceCollection"/>.</returns>
        IServiceCollection RegisterFrom(IServiceCollection serviceCollection);

        /// <summary>
        /// Registers internal scheduled services.
        /// </summary>
        /// <param name="serviceCollection">IoC collection</param>
        /// <returns>Returns the <see cref="IServiceCollection"/>.</returns>
        IServiceCollection RegisterScheduled(IServiceCollection serviceCollection);
    }
}