using Kyameru.Core.Entities;
using System;

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