using Kyameru.Core;

namespace Kyameru
{
    /// <summary>
    /// Core route builder utility.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Route"/> class from being created.
        /// </summary>
        private Route()
        {
        }

        /// <summary>
        /// Creates an instance of the route builder.
        /// </summary>
        /// <param name="componentUri">Valid Kyameru URI</param>
        /// <returns>Returns an instance of the <see cref="RouteBuilder"/> class.</returns>
        public static RouteBuilder From(string componentUri)
        {
            return new RouteBuilder(componentUri);
        }
    }
}