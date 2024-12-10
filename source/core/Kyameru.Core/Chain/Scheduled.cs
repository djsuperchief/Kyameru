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
    /// Scheduled base chain used to run component on specified schedule.
    /// </summary>
    public class Scheduled : BackgroundService
    {
        /// <summary>
        /// Main from component.
        /// </summary>
        private readonly IScheduleComponent fromComponent;

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
        /// Value indicating whether the component should bubble exceptions.
        /// </summary>
        private readonly bool raiseExceptions;

        /// <summary>
        /// Schedule to run on.
        /// </summary>
        private readonly Schedule schedule;

        /// <summary>
        /// Scheduler helper.
        /// </summary>
        private readonly Utils.Scheduler scheduler = new Utils.Scheduler(true);

        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// Instantiates a new instance of the Scheduled chain component.
        /// </summary>
        /// <param name="fromComponent">Scheduled from component.</param>
        /// <param name="next">Next component in the chain.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="id">Route Id.</param>
        /// <param name="isAtomicRoute">Obsolete (maybe).</param>
        /// <param name="raiseExceptions">Value indicating whether the route should bubble exceptions.</param>
        /// <param name="targetedSchedule">Targeted schedule</param>
        public Scheduled(
            IScheduleComponent fromComponent,
            IChain<Routable> next,
            ILogger logger,
            string id,
            bool isAtomicRoute,
            bool raiseExceptions,
            Schedule targetedSchedule)
        {
            this.fromComponent = fromComponent;
            this.fromComponent.Setup();
            this.next = next;
            this.logger = logger;
            this.fromComponent.OnLog += FromComponent_OnLog;
            identity = id;
            //IsAtomicRoute = ;
            fromComponent.OnActionAsync += FromComponent_OnActionAsync;
            this.raiseExceptions = raiseExceptions;
            schedule = targetedSchedule;
        }

        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="stoppingToken">Token for signalling the thread to stop.</param>
        /// <returns>Returns an async task.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                if (Utils.TimeProvider.Current.UtcNow >= scheduler.NextExecution)
                {
                    try
                    {
                        await fromComponent.RunAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        FromComponent_OnLog(this, new Log(LogLevel.Error, ex.Message, ex));
                        if (raiseExceptions)
                        {
                            throw;
                        }

                    }

                    scheduler.Next(schedule);
                }

                autoResetEvent.WaitOne(TimeSpan.FromSeconds(5));
            } while (!stoppingToken.IsCancellationRequested);
            logger.LogDebug("Scheduled component stopping");
        }

        /// <summary>
        /// Stops the component.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Returns a task.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            fromComponent.OnLog -= FromComponent_OnLog;
            fromComponent.OnActionAsync -= FromComponent_OnActionAsync;
            await base.StopAsync(cancellationToken);
        }

        private async Task FromComponent_OnActionAsync(object sender, RoutableEventData e)
        {
            await next?.HandleAsync(e.Data, e.CancellationToken);
        }

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
    }
}
