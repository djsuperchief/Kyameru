using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// Intermediary processing component.
    /// </summary>
    internal class Process : BaseChain
    {
        /// <summary>
        /// Core processing component.
        /// </summary>
        private readonly IProcessor component;

        /// <summary>
        /// Initializes a new instance of the <see cref="Process"/> class.
        /// </summary>
        /// <param name="logger">Logger class.</param>
        /// <param name="processComponent">Processing component.</param>
        /// <param name="identity">Identity of route.</param>
        public Process(ILogger logger, IProcessor processComponent, string identity) : base(logger, identity)
        {
            component = processComponent;
            component.OnLog += OnLog;
        }

        public override async Task HandleAsync(Routable routable, CancellationToken cancellationToken)
        {
            if (!routable.InError)
            {
                try
                {
                    await component.ProcessAsync(routable, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.KyameruException(identity, ex.Message, ex);
                    routable.SetInError(new Entities.Error("Processing component", "Handle", ex.Message));
                }
            }

            await base.HandleAsync(routable, cancellationToken);
        }
    }
}