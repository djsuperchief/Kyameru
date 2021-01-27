using System;
using Kyameru.Core.Entities;

namespace Kyameru.Tests.EntityTests
{
    public class BodyTests<T> : IBodyTests
    {
        private readonly T item;
        private readonly string expected;

        public BodyTests(T value, string expected)
        {
            item = value;
            this.expected = expected;
        }

        public bool IsEqual(Routable routable)
        {
            routable.SetBody<T>(item);
            return expected == routable.Headers["DataType"];
        }
    }
}