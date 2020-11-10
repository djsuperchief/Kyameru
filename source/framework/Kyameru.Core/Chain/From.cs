using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    internal class From : BackgroundService
    {
        private readonly IFromComponent fromComponent;
        private readonly IChain<Routable> next;
        private readonly ILogger logger;

        public From(IFromComponent fromComponent, IChain<Routable> next, ILogger logger)
        {
            this.fromComponent = fromComponent;
            this.fromComponent.Setup();
            this.fromComponent.OnAction += FromComponent_OnAction;
            this.next = next;
            this.logger = logger;
            this.fromComponent.OnLog += this.FromComponent_OnLog;
        }

        private void FromComponent_OnLog(object sender, Log e)
        {
            if (e.Error == null)
            {
                this.logger.Log(e.LogLevel, e.Message);
            }
            else
            {
                this.logger.LogError(e.Error, e.Message);
            }
        }

        private void FromComponent_OnAction(object sender, Entities.Routable e)
        {
            this.next?.Handle(e);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                this.fromComponent.Start();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, Resources.ERROR_FROM_COMPONENT);
            }

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            this.fromComponent.Stop();
            this.fromComponent.OnAction -= this.FromComponent_OnAction;
            await base.StopAsync(stoppingToken);
        }
    }
}