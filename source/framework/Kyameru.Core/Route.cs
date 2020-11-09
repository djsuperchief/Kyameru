using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Kyameru.Core;

namespace Kyameru
{
    /// <summary>
    /// Core route builder utility.
    /// </summary>
    public class Route
    {
        private Route(IFromComponent from)
        {
        }

        /// <summary>
        /// Creates a new route.
        /// </summary>
        /// <param name="from">A compatable from component.</param>
        /// <param name="args">Component arguments.</param>
        /// <returns>Returns a new instance of the <see cref="RouteBuilder"/> class.</returns>
        public static RouteBuilder From(string from, params string[] args)
        {
            return new RouteBuilder(from, args);
        }

        public static RouteBuilder From(string componentUri)
        {
            return new RouteBuilder(componentUri);
        }
    }
}