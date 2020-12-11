using System.Text.Json;

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
            this.Component = component;
            this.CurrentAction = action;
            this.Message = message;
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
    }
}