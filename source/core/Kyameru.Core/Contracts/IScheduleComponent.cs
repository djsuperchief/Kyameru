using System;

namespace Kyameru.Core.Contracts
{
    /// <summary>
    /// Schedule component
    /// </summary>
    public interface IScheduleComponent : IProcessComponent
    {
        /// <summary>
        /// Setup the component.
        /// </summary>
        void Setup();
    }
}
