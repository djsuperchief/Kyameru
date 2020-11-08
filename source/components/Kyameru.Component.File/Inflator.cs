using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.File
{
    public class Inflator : IOasis
    {
        public IFromComponent CreateComponent()
        {
            return new FileWatcher();
        }
    }
}
