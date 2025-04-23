namespace Kyameru.Core
{
    /// <summary>
    /// Common string resources for Kyameru.
    /// </summary>
    public static class Resources
    {
        internal const string INFO_SETTINGUPROUTE = "Setting up route...";
        internal const string INFO_PROCESSINGCOMPONENT = "Setting up processor {0}...";
        internal const string INFO_SETUP_TO = "Setting up to chain link {0}...";
        internal const string INFO_SETUP_ERR = "Setting up error processor {0}...";
        internal const string INFO_SETUP_ATOMIC = "Setting up atomic chain link {0}...";

        internal const string ERROR_ACTIVATION_FROM = "Error activating from chain link.";
        internal const string ERROR_ACTIVATION_TO = "Error activating to chain link.";
        internal const string ERROR_ACTIVATING_ATOMIC = "Error activating atomic chain link";
        internal const string ERROR_FROM_COMPONENT = "Error in starting from chain link, see inner exception.";
        internal const string ERROR_HEADER_IMMUTABLE = "Error in setting header, header already exists. Headers are immutable.";
        internal const string ERROR_HEADER_CALLBACK = "Error executing header callback. See inner exception.";
        internal const string ERROR_REGISTERING_SERVICES = "Error registering services. See inner exception.";
        internal const string ERROR_ROUTE_URI = "Error in constructing route from URI, see inner exception.";
        internal const string ERROR_HEADER_IMMUTABLE_ADDED = "Error adding immutable header {0}, already present.";
        internal const string ERROR_SETUP_COMPONENT_INVOCATION = "Error activating processor. No valid invocation.";

        internal const string ERROR_SCHEDULE_NOTSUPPORTED = "Component '{0}' does not support scheduling";

        /// <summary>
        /// Message for unavailable routes.
        /// </summary>
        public const string ERROR_ROUTE_UNAVAILABLE = "Error, Route '{0}' not available in component '{1}";

        internal const string WARNING_ROUTE_EXIT = "Route indicated for early exit. {0}";

        internal const string DEBUG_HEADER_DETERMINE = "Determining which header callback to use.";
        internal const string DEBUG_HEADER_RUNNING = "Running header callback.";

        internal const string ERROR_SCHEDULE_TIME_INVALID = "An invalid unit has been specified for schedule. Minutes 0-59, Hours 0-23";

        internal const string ERROR_SCHEDULE_ALREADY_DEFINED = "A schedule has already been defined.";

        internal const string ERROR_COMPONENT_NOT_FOUND = "Component not found {1}";
    }
}