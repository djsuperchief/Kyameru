using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Entities;
using Kyameru.Core.Exceptions;

namespace Kyameru.Component.Rest.Implementation
{
    public class HttpContentFactory : IHttpContentFactory
    {
        private static readonly Dictionary<string, Func<Routable, HttpContent>> ContentFactory =
            new Dictionary<string, Func<Routable, HttpContent>>()
            {
                { "application/json", JsonContent },
                { "text/json", JsonContent },
                { "text/plain", PlainText },
                { "application/octet-stream", ByteContent }
            };

        public HttpContent Create(Routable routable)
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

        private static HttpContent ByteContent(Routable routable)
        {
            var content = new ByteArrayContent(routable.Body as byte[]);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            
            return content;
        }
    }
}