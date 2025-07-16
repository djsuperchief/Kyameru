using System;

namespace Kyameru.Core
{
    public static class BuilderExtensions
    {
        public static Builder AuthWithApiToken(this Builder builder, string token)
        {

            return builder;
        }

        public static RouteBuilder AuthWithApiToken(this RouteBuilder routeBuilder, string token)
        {

            return routeBuilder;
        }

        public static Builder AuthWithApiToken(this Builder builder, string header, string token)
        {

            return builder;
        }

        public static RouteBuilder AuthWithApiToken(this RouteBuilder routeBuilder, string header, string token)
        {

            return routeBuilder;
        }
    }
}