using Kyameru.Core.Entities;

namespace Kyameru.Component.Sns;

public class SnsMessage
{
    public string Message { get; private set; }

    public string Arn { get; private set; }

    public static SnsMessage FromRoutable(Routable routable, Dictionary<string, string> componentHeaders)
    {
        return new SnsMessage()
        {
            Message = routable.Body.ToString(),
            Arn = routable.Headers.TryGetValue("SNSARN", componentHeaders["ARN"])
        };
    }
}
