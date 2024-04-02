using Amazon.S3;
using Kyameru.Core.Entities;

namespace Kyameru.Components.S3;

public class S3To : ITo
{
    private IAmazonS3 s3client;

    public event EventHandler<Log> OnLog;

    public S3To(IAmazonS3 client)
    {
        // TODO: Inject S3 config so we can override endpoint etc.
        s3client = client;
    }

    public void Process(Routable routable)
    {
        throw new NotImplementedException();
    }

    public Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {

    }

    public void SetHeaders(Dictionary<string, string> headers)
    {
        // TODO set headers
        throw new NotImplementedException();
    }
}
