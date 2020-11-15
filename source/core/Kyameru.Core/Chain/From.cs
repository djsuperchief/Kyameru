﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kyameru.Core.Chain
{
    /// <summary>
    /// From component.
    /// </summary>
    internal class From : BackgroundService
    {
        /// <summary>
        /// Main from component.
        /// </summary>
        private readonly IFromComponent fromComponent;

        /// <summary>
        /// Next processing component.
        /// </summary>
        private readonly IChain<Routable> next;

        /// <summary>
        /// Logger class.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="From"/> class.
        /// </summary>
        /// <param name="fromComponent">From component to use.</param>
        /// <param name="next">Next processing component.</param>
        /// <param name="logger">Logger class.</param>
        public From(IFromComponent fromComponent, IChain<Routable> next, ILogger logger)
        {
            this.fromComponent = fromComponent;
            this.fromComponent.Setup();
            this.fromComponent.OnAction += this.FromComponent_OnAction;
            this.next = next;
            this.logger = logger;
            this.fromComponent.OnLog += this.FromComponent_OnLog;
        }

        /// <summary>
        /// Stops the component.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Returns a task.</returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this.fromComponent.Stop();
            this.fromComponent.OnAction -= this.FromComponent_OnAction;
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Executes the component.
        /// </summary>
        /// <param name="stoppingToken">Stopping Token.</param>
        /// <returns>Returns a task.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                this.fromComponent.Start();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, Resources.ERROR_FROM_COMPONENT);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Logging event handler.
        /// </summary>
        /// <param name="sender">Class sending the event.</param>
        /// <param name="e">Log object.</param>
        private void FromComponent_OnLog(object sender, Log e)
        {
            if (e.Error == null)
            {
                this.logger.Log(e.LogLevel, e.Message);
            }
            else
            {
                this.logger.LogError(e.Error, e.Message);
            }
        }

        /// <summary>
        /// Event raised when the from component starts processing.
        /// </summary>
        /// <param name="sender">Class sending the event.</param>
        /// <param name="e">Message to send.</param>
        private void FromComponent_OnAction(object sender, Entities.Routable e)
        {
            this.next?.Handle(e);
        }
    }
}