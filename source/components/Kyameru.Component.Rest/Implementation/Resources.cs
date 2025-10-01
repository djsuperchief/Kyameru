using System;

namespace Kyameru.Component.Rest
{
    public class Resources
    {
        public const string ERROR_MISSINGHEADER = "Error, missing header '{0}'.";
        public const string ERROR_INVALID_METHOD = "Error, method '{0}' is not a valid Http method available in this component.";
        public const string ERROR_REQUEST = "A non successful response was received: {0}";
        public const string ERROR_CONTENT_TYPE = "The content type '{0}' is not valid for the Kyameru Rest Component.";

        public static string GetResource(string resource, params string[] args)
        {
            return string.Format(resource, args);
        }
    }
}