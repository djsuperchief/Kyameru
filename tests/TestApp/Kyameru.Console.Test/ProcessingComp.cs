using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Console.Test
{
    internal class ProcessingComp : IProcessComponent
    {
        public event EventHandler<Log> OnLog;

        public void Process(Routable routable)
        {
            routable.SetBody<string>("Farts are not food");
        }
    }
}
