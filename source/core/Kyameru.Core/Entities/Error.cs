namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Error entity.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="component">Name of the component.</param>
        /// <param name="action">Action being executed.</param>
        /// <param name="message">Error message.</param>
        public Error(string component, string action, string message)
        {
            Component = component;
            CurrentAction = action;
            Message = message;
        }

        /// <summary>
        /// Gets the component string.
        /// </summary>
        public string Component { get; private set; }

        /// <summary>
        /// Gets the executing action.
        /// </summary>
        public string CurrentAction { get; private set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the inner error.
        /// </summary>
        public Error InnerError { get; set; }
    }
}