﻿using Kyameru.Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kyameru.Core.BaseProcessors
{
    /// <summary>
    /// Add header component.
    /// </summary>
    public class AddHeader : IProcessor
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
        private Action<Routable> callbackOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHeader"/> class.
        /// </summary>
        /// <param name="header">Header value to add.</param>
        /// <param name="value">Value to assign for header.</param>
        public AddHeader(string header, string value)
        {
            this.header = header;
            this.value = value;
            callbackOption = SetValueHeader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHeader"/> class.
        /// </summary>
        /// <param name="header">Header value to add.</param>
        /// <param name="callbackOne">Value callback to assign for header.</param>
        public AddHeader(string header, Func<string> callbackOne)
        {
            this.header = header;
            callback = callbackOne;
            callbackOption = SetCallbackHeader;
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
            callbackOption = SetReturnableCallbackHeader;
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
            OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Debug, Resources.DEBUG_HEADER_DETERMINE));
            callbackOption(routable);
        }

        /// <summary>
        /// Process the incoming request.
        /// </summary>
        /// <param name="routable">Message to be processed.</param>
        /// <param name="cancellationToken">Thread cancellation token</param>
        public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Process(routable);
            }

            await Task.CompletedTask;
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

        private void SetValueHeader(Routable routable)
        {
            OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Debug, Resources.DEBUG_HEADER_RUNNING));
            routable.SetHeader(header, value);
        }

        private void SetCallbackHeader(Routable routable)
        {
            try
            {
                routable.SetHeader(header, callback());
            }
            catch (Exception ex)
            {
                routable.SetInError(SetError("Callback", Resources.ERROR_HEADER_CALLBACK));
                OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Error, Resources.ERROR_HEADER_CALLBACK, ex));
            }
        }

        private void SetReturnableCallbackHeader(Routable routable)
        {
            try
            {
                routable.SetHeader(header, callbackTwo(routable));
            }
            catch (Exception ex)
            {
                routable.SetInError(SetError("Callback", Resources.ERROR_HEADER_CALLBACK));
                OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Error, Resources.ERROR_HEADER_CALLBACK, ex));
            }
        }
    }
}