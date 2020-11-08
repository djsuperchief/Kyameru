using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Hosting;

namespace Kyameru.Core.Chain
{
    internal class From : BackgroundService
    {
        private readonly IFromComponent fromComponent;
        private readonly IChain<Routable> next;

        public From(IFromComponent fromComponent, string[] args, IChain<Routable> next)
        {
            this.fromComponent = fromComponent;
            this.fromComponent.Setup(args);
            this.fromComponent.OnAction += FromComponent_OnAction;
            this.next = next;
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
