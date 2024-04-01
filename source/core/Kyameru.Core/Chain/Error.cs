using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Kyameru.Tests")]

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// Error processing.
    /// </summary>

    internal class Error : BaseChain
    {
        /// <summary>
        /// Error component.
        /// </summary>
        private readonly IErrorComponent errorComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="logger">Logging interface.</param>
        /// <param name="errorComponent">Error component.</param>
        /// <param name="identity">Route identity.</param>
        public Error(ILogger logger, IErrorComponent errorComponent, string identity) : base(logger, identity)
        {
            this.errorComponent = errorComponent;
            this.errorComponent.OnLog += OnLog;
        }

        /// <summary>
        /// Passes processing to the next in the chain.
        /// </summary>
        /// <param name="item">Message to process.</param>
        public override void Handle(Routable item)
        {
            if (item.InError)
            {
                try
                {
                    errorComponent.Process(item);
                }
                catch (Exception ex)
                {
                    Logger.KyameruException(identity, ex.Message, ex);
                    item.SetInError(new Entities.Error("Error Component", "Handle", ex.Message));
                }
            }
        }

        public override async Task HandleAsync(Routable item, CancellationToken cancellationToken)
        {
            if (item.InError)
            {
                try
                {
                    await errorComponent.ProcessAsync(item, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.KyameruException(identity, ex.Message, ex);
                    item.SetInError(new Entities.Error("Error Component", "Handle", ex.Message));
                }
            }

            await base.HandleAsync(item, cancellationToken);
        }
    }
}