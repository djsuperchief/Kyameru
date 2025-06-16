using System.Collections.Generic;
using System.Linq;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Sns
{
    public class SnsMessage
    {
        public string Message { get; private set; }

        public string Arn { get; private set; }

        public Dictionary<string, string> Attributes { get; private set; }

        public static SnsMessage FromRoutable(Routable routable, Dictionary<string, string> componentHeaders)
        {
            var response = new SnsMessage
            {
                Message = routable.Body.ToString(),
                Arn = routable.Headers.TryGetValue("SNSARN", componentHeaders["ARN"])
            };

            response.Attributes = routable.Headers.ToDictionary(x => x.Key, x => x.Value);

            return response;
        }
    }
}