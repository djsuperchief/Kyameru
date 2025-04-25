using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Faker
{
    public class To : IFakerTo
    {
        private readonly IExtractor extractor;
        public event EventHandler<Log>? OnLog;

        public To(IExtractor messageExtractor)
        {
            extractor = messageExtractor;
        }
        public void Process(Routable routable)
        {
            extractor.SetRoutable(routable);
        }

        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            extractor.SetRoutable(routable);
            OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, "Fake To Route"));
            await Task.CompletedTask;
        }
    }
}