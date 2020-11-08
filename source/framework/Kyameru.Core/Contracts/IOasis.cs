using System;
namespace Kyameru.Core.Contracts
{
    public interface IOasis
    {
        IFromComponent CreateFromComponent(string[] args);

        IToComponent CreateToComponent(string[] args);
    }
}
