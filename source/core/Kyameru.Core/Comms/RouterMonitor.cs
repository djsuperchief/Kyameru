using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.Hosting;

namespace Kyameru.Core.Comms
{
    /// <summary>
    /// Router monitor.
    /// </summary>
    public class RouterMonitor : BackgroundService
    {
        private readonly IKRouter _router;
        
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// Instantiates a new instance of the <see cref="RouterMonitor"/> class.
        /// </summary>
        /// <param name="router">Kyameru Router.</param>
        public RouterMonitor(IKRouter router)
        {
            _router = router;
        }
        
        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _autoResetEvent.WaitOne(TimeSpan.FromSeconds(30));
            }
        }

        /// <inheritdoc/>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _router.CloseAll();
            await base.StopAsync(cancellationToken);
        }
    }
}