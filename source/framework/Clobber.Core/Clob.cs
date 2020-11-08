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

        public static RouteBuilder From<T>(T from) where T : Contracts.IFromComponent
        {
            return new RouteBuilder(from);
        }

        public static RouteBuilder From(Contracts.IFromComponent from, string[] args)
        {
            return new RouteBuilder(from, args);
        }

        public static RouteBuilder From(string from, params string[] args)
        {
            return new RouteBuilder(from, args);
        }
    }
}
