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

        public string Uri { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Uri))
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

                Uri = builder.ToString();
            }

            return Uri;
        }
    }
}