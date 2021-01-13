using Kyameru.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyameru.Component.Test
{
    public class Inflator : IOasis
    {
        public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
        {
            return new Atomic(headers);
        }

        public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic)
        {
            return new From(headers);
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers)
        {
            return new To(headers);
        }
    }
}