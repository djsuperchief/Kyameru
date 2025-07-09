using System;

namespace Kyameru.Tests.BuilderTests;

public class TestImplementation : ITestContract
{
    public string TestProp { get; private set; }

    public TestImplementation(string testProp = "Test")
    {
        TestProp = testProp;
    }

    public void DoSomething()
    {
        throw new NotImplementedException();
    }
}
