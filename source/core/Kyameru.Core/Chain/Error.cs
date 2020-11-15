﻿using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;
using System;

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// Error processing.
    /// </summary>
    internal class Error : BaseChain
    {
        /// <summary>
        /// Error component.
        /// </summary>
        private readonly IErrorComponent errorComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="logger">Logging interface.</param>
        /// <param name="errorComponent">Error component.</param>
        public Error(ILogger logger, IErrorComponent errorComponent) : base(logger)
        {
            this.errorComponent = errorComponent;
            this.errorComponent.OnLog += this.ErrorComponent_OnLog;
        }

        /// <summary>
        /// Passes processing to the next in the chain.
        /// </summary>
        /// <param name="item">Message to process.</param>
        public override void Handle(Routable item)
        {
            if (item.InError)
            {
                try
                {
                    this.errorComponent.Process(item);
                }
                catch (Exception ex)
                {
                    this.ErrorComponent_OnLog(this, new Log(LogLevel.Error, ex.Message, ex));
                }
            }
        }

        /// <summary>
        /// Logging event handler.
        /// </summary>
        /// <param name="sender">Class sending the event.</param>
        /// <param name="e">Log object.</param>
        private void ErrorComponent_OnLog(object sender, Log e)
        {
            if (e.Error == null)
            {
                this.Logger.Log(e.LogLevel, e.Message);
            }
            else
            {
                this.Logger.LogError(e.Error, e.Message);
            }
        }
    }
}