using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.Error
{
    public abstract class BaseError
    {
        protected readonly Dictionary<string, string> headers;

        public BaseError(Dictionary<string, string> headers)
        {
            this.headers = headers;
        }

        protected bool WillError()
        {
            bool error = false;
            if (bool.TryParse(this.headers["Error"], out error))
            {
                // do nothing
            }

            return error;
        }
    }
}