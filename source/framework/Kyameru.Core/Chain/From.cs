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
        }

        private void FromComponent_OnAction(object sender, Entities.Routable e)
        {
            this.next?.Handle(e);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.fromComponent.Start();

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