using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Kyameru.Component.Ftp.Settings
{
    /// <summary>
    /// Ftp settings entity
    /// </summary>
    public class FtpSettings
    {
        /// <summary>
        /// Gets the Ftp host.
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Gets the Ftp path.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the ftp port.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets the Ftp poll time.
        /// </summary>
        public int PollTime { get; private set; }

        /// <summary>
        /// Gets the file filter.
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// Gets the networking credentials.
        /// </summary>
        public NetworkCredential Credentials { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a recursive search should take place.
        /// </summary>
        public bool Recursive { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether files should be deleted off the Ftp endpoint after downloading.
        /// </summary>
        public bool Delete { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpSettings"/> class.
        /// </summary>
        /// <param name="headers"></param>
        public FtpSettings(Dictionary<string, string> headers)
        {
            this.Host = headers["Host"];
            this.Path = headers["Target"];
            this.Port = int.Parse(headers["Port"]);
            this.PollTime = int.Parse(headers["PollTime"]);
            this.Filter = headers["Filter"];
            this.Credentials = this.GetCredentials(headers.GetKeyValue("UserName"), headers.GetKeyValue("Password"));
            this.Recursive = bool.Parse(headers["Recursive"]);
            this.Delete = bool.Parse(headers["Delete"]);
        }

        /// <summary>
        /// Gets the network credentials.
        /// </summary>
        /// <param name="username">Username to convert.</param>
        /// <param name="password">Password to convert.</param>
        /// <returns>Returns network credentials.</returns>
        private NetworkCredential GetCredentials(string username, string password)
        {
            NetworkCredential response = null;
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                response = new NetworkCredential(username, password);
            }
            else if (!string.IsNullOrWhiteSpace(username))
            {
                response = new NetworkCredential("anonymous", username);
            }

            return response;
        }
    }
}