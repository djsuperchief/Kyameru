using System;
using System.Collections.Generic;
using System.Linq;

namespace Kyameru.Tests.Extensions;

public static class DictionaryExtensions
{
    public static List<string> ToAssertable(this IEnumerable<KeyValuePair<string, string>> dictionary) =>
        dictionary.OrderBy(x => x.Key).Select(x => $"{x.Key}:{x.Value}").ToList();

}
