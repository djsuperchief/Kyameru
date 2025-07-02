using System;
using System.Collections.Generic;

namespace Kyameru.Core.Entities
{
    /// <summary>
    /// Class to hold chain dependencies until registration time.
    /// </summary>
    internal class ChainDependency
    {
        public Guid Id { get; private set; }
        public Type Contract { get; private set; }
        public Type Implementation { get; private set; }

        public ChainDependency(Guid id, Type contract, Type implementation)
        {
            Id = id;
            Contract = contract;
            Implementation = implementation;
        }
    }
}