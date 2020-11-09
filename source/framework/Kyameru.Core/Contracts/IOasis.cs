using System;
using System.Collections.Generic;

namespace Kyameru.Core.Contracts
{
    public interface IOasis
    {
        IFromComponent CreateFromComponent(string[] args);

        IFromComponent CreateFromComponent(Dictionary<string, string> headers);

        IToComponent CreateToComponent(string[] args);

        IToComponent CreateToComponent(Dictionary<string, string> headers);
    }
}