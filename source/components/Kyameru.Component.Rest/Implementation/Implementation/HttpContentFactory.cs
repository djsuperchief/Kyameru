using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;

namespace Kyameru.Component.Rest.Implementation
{
    public class HttpContentFactory
    {
        private static readonly Dictionary<string, Func<Routable, HttpContent>> ContentFactory =
            new Dictionary<string, Func<Routable, HttpContent>>()
            {
                { "application/json", JsonContent },
                { "text/json", JsonContent },
                { "text/plain", PlainText }
            };

        public static HttpContent Create(Routable routable)
        {
            if (!ContentFactory.ContainsKey(routable.Headers["HttpContentType"]))
            {
                throw new ComponentException(Resources.GetResource(Resources.ERROR_CONTENT_TYPE, routable.Headers["HttpContentType"]));
            }
            
            return ContentFactory[routable.Headers["HttpContentType"]](routable);
        }
        
        private static HttpContent JsonContent(Routable routable)
        {
            return new StringContent(routable.Body as string, Encoding.UTF8, "application/json");
        }

        private static HttpContent PlainText(Routable routable)
        {
            return new StringContent(routable.Body as string, Encoding.UTF8, "text/plain");
        }
    }
}