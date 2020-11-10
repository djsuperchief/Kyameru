using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kyameru.Testable.Service.DummyComponents
{
    public class DummyError : IProcessComponent
    {
        public event EventHandler<Log> OnLog;

        public void Process(Routable routable)
        {
            throw new NotImplementedException("This is meant to throw an error");
        }
    }
}