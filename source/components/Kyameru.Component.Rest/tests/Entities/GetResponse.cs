using System;

namespace Kyameru.Component.Rest.Tests.Entities;

public class GetResponse
{
    public string Method { get; set; }

    public Dictionary<string, string> QueryStringParameters { get; set; }

    public string Url { get; set; }
}
