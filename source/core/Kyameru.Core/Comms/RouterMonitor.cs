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

        /// <summary>
        /// Instantiates a new instance of the <see cref="RouterMonitor"/> class.
        /// </summary>
        /// <param name="router">Kyameru Router.</param>
        public RouterMonitor(IKRouter router)
        {
            _router = router;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _router.CloseAll();
            await base.StopAsync(cancellationToken);
        }
    }
}