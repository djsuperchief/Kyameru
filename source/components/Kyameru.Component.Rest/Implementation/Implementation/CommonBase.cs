using System;
using System.Collections.Generic;
using Kyameru.Component.Rest.Extensions;

namespace Kyameru.Component.Rest.Implementation
{
    public abstract class CommonBase
    {
        protected Dictionary<string, string> _headers;
        protected readonly string[] _requiredHeaders = new string[]
        {
            "endpoint",
            "Host",
            "Target"
        };

        protected readonly string[] _validMethods = new string[]
        {
            "post",
            "get",
            "put",
            "delete"
        };


        public string HttpMethod { get; protected set; } = "get";

        public string Url { get; protected set; }

        protected void SetUrl()
        {
            Url = _headers.ToValidApiEndpoint();
        }
    }
}