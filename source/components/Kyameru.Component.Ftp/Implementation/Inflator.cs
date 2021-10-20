using Kyameru.Core.Contracts;
using System.Collections.Generic;

namespace Kyameru.Component.Ftp
{
    /// <summary>
    /// Initiator class.
    /// </summary>
    public class Inflator : IOasis
    {
        public IFromComponent CreateFromComponent(Dictionary<string, string> headers)
        {
            return new From(headers, new Components.WebRequestUtility());
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers)
        {
            return new To(headers, new Components.WebRequestUtility());
        }
    }
}