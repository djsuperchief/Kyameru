namespace Kyameru.Core
{
    internal static class Resources
    {
        internal const string INFO_SETTINGUPROUTE = "Setting up route...";
        internal const string INFO_PROCESSINGCOMPONENT = "Setting up processing component {0}...";
        internal const string INFO_SETUP_TO = "Setting up to component {0}...";
        internal const string INFO_SETUP_ERR = "Setting up error component {0}...";

        internal const string ERROR_ACTIVATION_FROM = "Error activating from component.";
        internal const string ERROR_ACTIVATION_TO = "Error activating to component.";
        internal const string ERROR_ACTIVATING_ATOMIC = "Error activating atomic component";
        internal const string ERROR_FROM_COMPONENT = "Error in starting from component, see inner exception.";
        internal const string ERROR_HEADER_IMMUTABLE = "Error in setting header, header already exists. Headers are immutable.";
        internal const string ERROR_HEADER_CALLBACK = "Error executing header callback. See inner exception.";
    }
}