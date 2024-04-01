using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Kyameru.Core;
using Kyameru.Core.Sys;
using Timer = System.Timers.Timer;

namespace Kyameru.Component.Ftp
{
    /// <summary>
    /// FTP From Component
    /// </summary>
    /// <remarks>
    /// Async not really implemented, needs more work and should really be using sftp.
    /// </remarks>
    public class From : IFtpFrom
    {
        private readonly FtpSettings ftpSettings;
        private readonly IWebRequestUtility webRequestUtility;
        private FtpClient ftp;

        private Timer poller;
        private bool isAsync = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="From"/> class.
        /// </summary>
        /// <param name="headers"></param>
        public From(Dictionary<string, string> headers, IWebRequestUtility webRequestUtility)
        {
            ftpSettings = new FtpSettings(headers.ToFromConfig());
            this.webRequestUtility = webRequestUtility;
        }

        public bool PollerIsActive => poller.Enabled;

        /// <summary>
        /// Event raised on action
        /// </summary>
        public event EventHandler<Routable> OnAction;

        public event AsyncEventHandler<RoutableEventData> OnActionAsync;

        /// <summary>
        /// Event raised when logging.
        /// </summary>
        public event EventHandler<Log> OnLog;

        /// <summary>
        /// Sets up the component.
        /// </summary>
        public void Setup()
        {
            ftp = new FtpClient(ftpSettings, webRequestUtility);
            poller = new Timer(ftpSettings.PollTime);
            poller.Elapsed += Poller_Elapsed;
            poller.AutoReset = true;
            ftp.OnDownloadFile += Ftp_OnDownloadFile;
            ftp.OnLog += Ftp_OnLog;
            ftp.OnError += Ftp_OnError;
            webRequestUtility.OnLog += WebRequestUtility_OnLog;
        }

        /// <summary>
        /// Event triggered when timer elapses.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Elapsed arguments.</param>
        private void Poller_Elapsed(object sender, ElapsedEventArgs e)
        {
            ftp.Poll();
        }

        /// <summary>
        /// Logs an error event.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Exception being raised.</param>
        private void Ftp_OnError(object sender, Exception e)
        {
            OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Error, Resources.ERROR_FTPPROCESSING, e));
        }

        /// <summary>
        /// Event raised when logging.
        /// </summary>
        /// <param name="sender">Object sending</param>
        /// <param name="e">Event message.</param>
        private void Ftp_OnLog(object sender, string e)
        {
            OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, e));
        }

        /// <summary>
        /// Event raised when file is downloaded.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Message to route.</param>
        private void Ftp_OnDownloadFile(object sender, Routable e)
        {
            if (isAsync)
            {
                OnActionAsync?.Invoke(this,new RoutableEventData(e, new CancellationToken()));
            }
            else
            {
                OnAction?.Invoke(this, e);    
            }
            
        }

        /// <summary>
        /// Starts the component.
        /// </summary>
        public void Start()
        {
            ftp.Poll();
            poller.Start();
        }

        /// <summary>
        /// Stops the component.
        /// </summary>
        public void Stop()
        {
            poller.Stop();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                isAsync = true;
                ftp.Poll();
                poller.Start();
            }

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            poller.Stop();
            await Task.CompletedTask;
        }

        private void WebRequestUtility_OnLog(object sender, string e)
        {
            OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, e));
        }

    }
}