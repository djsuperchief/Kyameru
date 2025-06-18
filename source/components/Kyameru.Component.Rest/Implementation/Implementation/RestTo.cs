using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Component.Rest.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Component.Rest.Implementation
{
    public class RestTo : CommonBase, IRestTo
    {
        public event EventHandler<Log> OnLog;

        public Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
            ValidateHeaders();
        }

        private void ValidateHeaders()
        {
            foreach (var required in _requiredHeaders)
            {
                if (!_headers.ContainsKey(required) || string.IsNullOrWhiteSpace(_headers[required]))
                {
                    throw new Core.Exceptions.ComponentException(string.Format(Resources.ERROR_MISSINGHEADER, required));
                }
            }

            if (_headers.ContainsKey("method") && _validMethods.Any(x => x == _headers["method"]))
            {
                HttpMethod = _headers["method"];
            }

            SetUrl();
        }
    }
}