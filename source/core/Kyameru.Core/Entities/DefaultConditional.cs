using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Core
{
    sealed class DefaultConditional : IConditionalProcessor
    {
        private Func<Routable, bool> _condition;

        public DefaultConditional(Func<Routable, bool> condition)
        {
            _condition = condition;
        }
        public bool Execute(Routable routable)
        {
            return _condition(routable);
        }
    }
}
