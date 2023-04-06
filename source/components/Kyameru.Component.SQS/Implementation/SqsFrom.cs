using System;
using Kyameru.Core.Contracts;
using Kyameru.Core.Entities;

namespace Kyameru.Component.SQS
{
    public class SqsFrom : IFromComponent
    {

        public event EventHandler<Routable> OnAction;
        public event EventHandler<Log> OnLog;

        public void Setup()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}

