using Kyameru.Component.Ftp.Contracts;
using Kyameru.Component.Ftp.Settings;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Timers;

namespace Kyameru.Component.Ftp
{
    /// <summary>
    /// FTP From Component
    /// </summary>
    public class From : IFtpFrom
    {
        private readonly FtpSettings ftpSettings;
        private readonly IWebRequestUtility webRequestUtility;
        private FtpClient ftp;

        private Timer poller;

        /// <summary>
        /// Initializes a new instance of the <see cref="From"/> class.
        /// </summary>
        /// <param name="headers"></param>
        public From(Dictionary<string, string> headers, IWebRequestUtility webRequestUtility)
        {
            this.ftpSettings = new FtpSettings(headers.ToFromConfig());
            this.webRequestUtility = webRequestUtility;
        }

        public bool PollerIsActive => this.poller.Enabled;

        /// <summary>
        /// Event raised on action
        /// </summary>
        public event EventHandler<Routable> OnAction;

        /// <summary>
        /// Event raised when logging.
        /// </summary>
        public event EventHandler<Log> OnLog;

        /// <summary>
        /// Sets up the component.
        /// </summary>
        public void Setup()
        {
            this.ftp = new FtpClient(this.ftpSettings, this.webRequestUtility);
            this.poller = new Timer(this.ftpSettings.PollTime);
            this.poller.Elapsed += this.Poller_Elapsed;
            this.poller.AutoReset = true;
            this.ftp.OnDownloadFile += this.Ftp_OnDownloadFile;
            this.ftp.OnLog += this.Ftp_OnLog;
            this.ftp.OnError += this.Ftp_OnError;
            this.webRequestUtility.OnLog += WebRequestUtility_OnLog;
        }

        /// <summary>
        /// Event triggered when timer elapses.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Elapsed arguments.</param>
        private void Poller_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.ftp.Poll();
        }

        /// <summary>
        /// Logs an error event.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Exception being raised.</param>
        private void Ftp_OnError(object sender, Exception e)
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Error, Resources.ERROR_FTPPROCESSING, e));
        }

        /// <summary>
        /// Event raised when logging.
        /// </summary>
        /// <param name="sender">Object sending</param>
        /// <param name="e">Event message.</param>
        private void Ftp_OnLog(object sender, string e)
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, e));
        }

        /// <summary>
        /// Event raised when file is downloaded.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Message to route.</param>
        private void Ftp_OnDownloadFile(object sender, Routable e)
        {
            this.OnAction?.Invoke(this, e);
        }

        /// <summary>
        /// Starts the component.
        /// </summary>
        public void Start()
        {
            this.ftp.Poll();
            this.poller.Start();
        }

        /// <summary>
        /// Stops the component.
        /// </summary>
        public void Stop()
        {
            this.poller.Stop();
        }
        private void WebRequestUtility_OnLog(object sender, string e)
        {
            this.OnLog?.Invoke(this, new Log(Microsoft.Extensions.Logging.LogLevel.Information, e));
        }

    }
}