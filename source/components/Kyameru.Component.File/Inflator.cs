using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.File
{
    public class Inflator : IOasis
    {
        public IFromComponent CreateFromComponent(Dictionary<string, string> headers)
        {
            return new FileWatcher(headers);
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers)
        {
            return new FileTo(headers);
        }
    }
}