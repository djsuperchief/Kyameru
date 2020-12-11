using Kyameru.Core.Entities;
using System;

namespace Kyameru.Core.BaseComponents
{
    /// <summary>
    /// Add header component.
    /// </summary>
    public class AddHeader : IProcessComponent
    {
        /// <summary>
        /// Header value
        /// </summary>
        private readonly string header;

        /// <summary>
        /// Value for header
        /// </summary>
        private readonly string value;

        /// <summary>
        /// Callback for header value
        /// </summary>
        private readonly Func<string> callback = null;

        /// <summary>
        /// Callback for header value
        /// </summary>
        private readonly Func<Routable, string> callbackTwo = null;

        /// <summary>
        /// Option to hold which callback to use.
        /// </summary>
        private readonly int callbackOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHeader"/> class.
        /// </summary>
        /// <param name="header">Header value to add.</param>
        /// <param name="value">Value to assign for header.</param>
        public AddHeader(string header, string value)
        {
            this.header = header;
            this.value = value;
            this.callbackOption = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHeader"/> class.
        /// </summary>
        /// <param name="header">Header value to add.</param>
        /// <param name="callbackOne">Value callback to assign for header.</param>
        public AddHeader(string header, Func<string> callbackOne)
        {
            this.header = header;
            this.callback = callbackOne;
            this.callbackOption = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHeader"/> class.
        /// </summary>
        /// <param name="header">Header value to add.</param>
        /// <param name="callbackTwo">Value callback to assign for header.</param>
        public AddHeader(string header, Func<Routable, string> callbackTwo)
        {
            this.header = header;
            this.callbackTwo = callbackTwo;
            this.callbackOption = 2;
        }

        /// <summary>
        /// Log event
        /// </summary>
        public event EventHandler<Log> OnLog;

        /// <summary>
        /// Process the incoming message.
        /// </summary>
        /// <param name="routable">Routable message.</param>
        public void Process(Routable routable)
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Debug, Resources.DEBUG_HEADER_DETERMINE));
            switch (this.callbackOption)
            {
                case 0:
                    this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Debug, Resources.DEBUG_HEADER_RUNNING));
                    routable.AddHeader(this.header, this.value);
                    break;

                case 1:
                    try
                    {
                        routable.AddHeader(this.header, this.callback());
                    }
                    catch (Exception ex)
                    {
                        routable.SetInError(this.SetError("Callback", Resources.ERROR_HEADER_CALLBACK));
                        this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Error, Resources.ERROR_HEADER_CALLBACK, ex));
                    }

                    break;

                case 2:
                    try
                    {
                        routable.AddHeader(this.header, this.callbackTwo(routable));
                    }
                    catch (Exception ex)
                    {
                        routable.SetInError(this.SetError("Callback", Resources.ERROR_HEADER_CALLBACK));
                        this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Error, Resources.ERROR_HEADER_CALLBACK, ex));
                    }

                    break;
            }
        }

        /// <summary>
        /// Sets the error object.
        /// </summary>
        /// <param name="action">Action performed.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Returns an instance of the <see cref="Error"/> class.</returns>
        private Error SetError(string action, string message)
        {
            return new Error("AddHeader", action, message);
        }
    }
}