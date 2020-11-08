using System;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.File
{
    public class Inflator : IOasis
    {
        
        public IFromComponent CreateFromComponent(string[] args)
        {
            return new FileWatcher(args);
        }

        public IToComponent CreateToComponent(string[] args)
        {
            return new FileTo(args);
        }
    }
}
