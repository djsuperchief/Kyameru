using System;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Object to hold references for chain link dependencies.
    /// </summary>
    public class ChainLinkDependency
    {
        /// <summary>
        /// Gets or sets the Id of the dependency.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the dependency type.
        /// </summary>
        public Type DependencyType { get; set; }
        
        /// <summary>
        /// Gets or sets the chain link for the dependency.
        /// </summary>
        public Enums.ChainLinkDependencyType ChainLink { get; set; }
    }
}