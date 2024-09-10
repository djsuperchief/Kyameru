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
    public class Cron : BackgroundService
    {
        /// <summary>
        /// Main from component.
        /// </summary>
        private readonly ICronComponent cronComponent;

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
        private readonly bool raiseExceptions;


        /// <summary>
        /// Initializes a new instance of the <see cref="From"/> class.
        /// </summary>
        /// <param name="cronComponent">Cron component to use.</param>
        /// <param name="next">Next processing component.</param>
        /// <param name="logger">Logger class.</param>
        /// <param name="id">Identity of the route.</param>
        /// <param name="isAtomicRoute">Value indicating whether the route is atomic.</param>
        /// <param name="raiseExceptions">Value indicating that the route should throw route exceptions up</param>
        public Cron(ICronComponent cronComponent, IChain<Routable> next, ILogger logger, string id, bool isAtomicRoute, bool raiseExceptions)
        {
            this.cronComponent = cronComponent;
            this.cronComponent.Setup();
            this.next = next;
            this.logger = logger;
            this.cronComponent.OnLog += CronComponent_OnLog;
            identity = id;
            IsAtomicRoute = isAtomicRoute;
            cronComponent.OnActionAsync += CronComponent_OnActionAsync;
            this.raiseExceptions = raiseExceptions;
        }

        /// <summary>
        /// Executes the Cron component.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token.</param>
        /// <returns>Returns a <see cref="Task" /> representing the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {

                await cronComponent.ExecuteAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Do nothing, this is expected.
            }
            catch (Exception ex)
            {
                CronComponent_OnLog(this, new Log(LogLevel.Error, ex.Message, ex));
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
        private void CronComponent_OnLog(object sender, Log e)
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

        private async Task CronComponent_OnActionAsync(object sender, RoutableEventData e)
        {
            await next?.HandleAsync(e.Data, e.CancellationToken);
        }
    }
}
