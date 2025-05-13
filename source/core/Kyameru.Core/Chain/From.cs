using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
using Kyameru.Core.Sys;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// From component.
    /// </summary>
    internal class From : BackgroundService
    {
        /// <summary>
        /// Main from component.
        /// </summary>
        private readonly IFromChainLink fromComponent;

        /// <summary>
        /// Next processing component.
        /// </summary>
        private readonly IChain<Routable> next;

        /// <summary>
        /// Logger class.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Route identity.
        /// </summary>
        private readonly string identity;

        private readonly bool raiseExceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="From"/> class.
        /// </summary>
        /// <param name="fromComponent">From component to use.</param>
        /// <param name="next">Next processing component.</param>
        /// <param name="logger">Logger class.</param>
        /// <param name="id">Identity of the route.</param>
        /// <param name="raiseExceptions">Value indicating that the route should throw route exceptions up</param>
        public From(IFromChainLink fromComponent, IChain<Routable> next, ILogger logger, string id, bool raiseExceptions)
        {
            this.fromComponent = fromComponent;
            this.fromComponent.Setup();
            this.next = next;
            this.logger = logger;
            this.fromComponent.OnLog += FromComponent_OnLog;
            identity = id;
            fromComponent.OnActionAsync += FromComponent_OnActionAsync;
            this.raiseExceptions = raiseExceptions;
        }



        /// <summary>
        /// Stops the component.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Returns a task.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await fromComponent.StopAsync(cancellationToken);
            fromComponent.OnLog -= FromComponent_OnLog;
            fromComponent.OnActionAsync -= FromComponent_OnActionAsync;
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Executes the component.
        /// </summary>
        /// <param name="stoppingToken">Stopping Token.</param>
        /// <returns>Returns a task.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await fromComponent.StartAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                FromComponent_OnLog(this, new Log(LogLevel.Error, ex.Message, ex));
                if (raiseExceptions)
                {
                    throw;
                }

            }
        }

        /// <summary>
        /// Logging event handler.
        /// </summary>
        /// <param name="sender">Class sending the event.</param>
        /// <param name="e">Log object.</param>
        private void FromComponent_OnLog(object sender, Log e)
        {
            if (e.Error == null)
            {
                logger.KyameruLog(identity, e.Message, e.LogLevel);
            }
            else
            {
                logger.KyameruException(identity, e.Message, e.Error);
            }
        }

        private async Task FromComponent_OnActionAsync(object sender, RoutableEventData e)
        {
            await next?.HandleAsync(e.Data, e.CancellationToken);
        }
    }
}