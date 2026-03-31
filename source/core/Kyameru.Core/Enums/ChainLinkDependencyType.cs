namespace Kyameru.Core.Enums
{
    /// <summary>
    /// Chain link dependency type
    /// </summary>
    public enum ChainLinkDependencyType
    {
        /// <summary>
        /// Unset
        /// </summary>
        /// <remarks>
        /// Must be set at route build time.
        /// </remarks>
        Unset,
        
        /// <summary>
        /// From Chain link
        /// </summary>
        From,
        
        /// <summary>
        /// To chain link
        /// </summary>
        To
    }
}