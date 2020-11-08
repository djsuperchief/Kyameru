using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;
using Kyameru.Core;

namespace Kyameru
{
    public class Route
    {
        private Route(IFromComponent from)
        {
        }

        public static RouteBuilder From(string from, params string[] args)
        {
            return new RouteBuilder(from, args);
        }
    }
}
