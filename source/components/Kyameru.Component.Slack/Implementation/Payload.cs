namespace Kyameru.Component.Slack
{
    /// <summary>
    /// Slack message payload
    /// </summary>
    internal class Payload
    {
        /// <summary>
        /// Gets or sets the slack message.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Gets or sets the slack username to use.
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// Gets or sets the slack channel to use.
        /// </summary>
        public string channel { get; set; }
    }
}