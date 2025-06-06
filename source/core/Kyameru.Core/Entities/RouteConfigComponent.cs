using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Route config component.
    /// </summary>
    public class RouteConfigComponent
    {
        /// <summary>
        /// Gets or sets the component type.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Gets of sets the post-processing component.
        /// </summary>
        public string PostProcess { get; set; }

        /// <summary>
        /// Gets or sets the path for the component.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the headers of the component.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the URI of the component.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the conditional component for a To route.
        /// </summary>
        public string When { get; set; }

        /// <summary>
        ///  Gets the registration type.
        /// </summary>
        [JsonIgnore]
        public Enums.ConfigToRegistrationType RegistrationType
        {
            get
            {
                var binary = $"{BinaryCheck(When)}{BinaryCheck(PostProcess)}";
                return (Enums.ConfigToRegistrationType)Convert.ToInt32(binary, 2);
            }
        }

        /// <summary>
        /// Gets the components full Kyameru URI
        /// </summary>
        /// <returns></returns>
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

        private int BinaryCheck(string item)
        {
            return Convert.ToInt32(!string.IsNullOrWhiteSpace(item));
        }
    }
}