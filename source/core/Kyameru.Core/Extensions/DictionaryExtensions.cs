using System.Collections.Generic;
using System.Linq;

namespace Kyameru.Core.Extensions
{
    internal static class DictionaryExtensions
    {
        public static Dictionary<string, string> GetImmutableValues(this Dictionary<string, string> kvp)
        {
            return kvp.Where(x => x.Key[..1] == "&").ToDictionary(x => x.Key[1..], x => x.Value);
        }

        public static Dictionary<string, string> GetMutableValues(this Dictionary<string, string> kvp)
        {
            return kvp.Where(x => x.Key[..1] != "&").ToDictionary(x => x.Key, x => x.Value);
        }

        public static void AddRange(this Dictionary<string, string> kvp, Dictionary<string, string> toAdd)
        {
            foreach (string key in toAdd.Keys)
            {
                if (!kvp.ContainsKey(key))
                {
                    kvp.Add(key, toAdd[key]);
                }
            }
        }
    }
}