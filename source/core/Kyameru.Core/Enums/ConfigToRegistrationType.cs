using System;

namespace Kyameru.Core.Enums
{
    /// <summary>
    /// Config To Route Registration Type
    /// </summary>
    public enum ConfigToRegistrationType
    {
        /// <summary>
        /// Standard When registration
        /// </summary>
        When = 2,

        /// <summary>
        /// When with post registration.
        /// </summary>
        WhenWithPost = 3,

        /// <summary>
        /// Standard To registration.
        /// </summary>
        To = 0,

        /// <summary>
        /// To with post registration.
        /// </summary>
        ToWithPost = 1
    }
}
