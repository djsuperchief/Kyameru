using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Tests;

public class ConditionalComponent : IConditionalProcessor
{
    public bool Execute(Routable routable)
    {
        routable.SetHeader("CondComp", "true");
        return routable.Body as string == "CondPass";
    }
}
