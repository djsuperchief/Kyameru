using System;
using Kyameru.Core.Entities;

namespace Kyameru.Testable.Service.DummyComponents
{
    public class Something : Kyameru.IProcessComponent
    {
        public Something()
        {
        }

        public void LogCritical(string critical)
        {
            throw new NotImplementedException();
        }

        public void LogError(string error)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(string info)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string warning)
        {
            throw new NotImplementedException();
        }

        public void Process(Routable routable)
        {
            string test = routable.Headers["DataType"];
            string helloworld = "hello world";
        }
    }
}