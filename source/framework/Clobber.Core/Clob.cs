using System;
using System.Collections.Generic;

namespace Kyameru.Core
{
    public class Route
    {
        private Route(Contracts.IFromComponent from)
        {
        }

        public static RouteBuilder From(Contracts.IFromComponent from)
        {
            return new RouteBuilder(from);
        }

        public static RouteBuilder From(Contracts.IFromComponent from, string[] args)
        {
            return new RouteBuilder(from, args);
        }
    }
}
