using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyameru.Core.Entities
{
    public class RouteConfigComponent
    {
        public string Component { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{Component.ToLower()}///");
            builder.Append($"{Path}?");
            var lastHeader = Headers.Last();
            foreach (var header in Headers.Keys)
            {
                builder.Append($"{header}={Headers[header]}");
                if (lastHeader.Key != header)
                {
                    builder.Append("&");
                }
            }

            return builder.ToString();
        }
    }
}