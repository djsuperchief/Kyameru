using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Extensions;
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
        private readonly IFromComponent fromComponent;

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

        /// <summary>
        /// value indicating whether the route will be atomic.
        /// </summary>
        private readonly bool IsAtomicRoute;
        private readonly bool isAsync;
        private readonly bool raiseExceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="From"/> class.
        /// </summary>
        /// <param name="fromComponent">From component to use.</param>
        /// <param name="next">Next processing component.</param>
        /// <param name="logger">Logger class.</param>
        /// <param name="id">Identity of the route.</param>
        /// <param name="isAtomicRoute">Value indicating whether the route is atomic.</param>
        /// <param name="isAsync">Value indicating that the route should be executed async</param>
        /// <param name="raiseExceptions">Value indicating that the route should throw route exceptions up</param>
        public From(IFromComponent fromComponent, IChain<Routable> next, ILogger logger, string id, bool isAtomicRoute, bool isAsync, bool raiseExceptions)
        {
            this.fromComponent = fromComponent;
            this.fromComponent.Setup();
            this.fromComponent.OnAction += this.FromComponent_OnAction;
            this.next = next;
            this.logger = logger;
            this.fromComponent.OnLog += this.FromComponent_OnLog;
            this.identity = id;
            this.IsAtomicRoute = isAtomicRoute;
            this.isAsync = isAsync;
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
            if (isAsync)
            {
                await fromComponent.StopAsync(cancellationToken);
            }
            else
            {
                this.fromComponent.Stop();
            }

            this.fromComponent.OnAction -= this.FromComponent_OnAction;
            this.fromComponent.OnLog -= FromComponent_OnLog;
            this.fromComponent.OnActionAsync -= FromComponent_OnActionAsync;
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
                if (isAsync)
                {
                    await fromComponent.StartAsync(stoppingToken);
                }
                else
                {
                    this.fromComponent.Start();
                }

            }
            catch (Exception ex)
            {
                this.FromComponent_OnLog(this, new Log(LogLevel.Error, ex.Message, ex));
                if (this.raiseExceptions)
                {
                    throw;
                }

            }

            await Task.CompletedTask;
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
                this.logger.KyameruLog(this.identity, e.Message, e.LogLevel);
            }
            else
            {
                this.logger.KyameruException(this.identity, e.Message, e.Error);
            }
        }

        /// <summary>
        /// Event raised when the from component starts processing.
        /// </summary>
        /// <param name="sender">Class sending the event.</param>
        /// <param name="e">Message to send.</param>
        private void FromComponent_OnAction(object sender, Entities.Routable e)
        {
            this.next?.Handle(e);
        }

        private async Task FromComponent_OnActionAsync(object sender, RoutableEventData e)
        {
            await next?.HandleAsync(e.Data, e.CancellationToken);
        }
    }
}