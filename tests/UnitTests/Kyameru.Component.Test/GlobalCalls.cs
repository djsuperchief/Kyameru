using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyameru.Component.Test
{
    public static class GlobalCalls
    {
        //public static List<string> Calls = new List<string>();
        public static Dictionary<string, List<string>> CallDict { get; set; }

        public static void AddCall(string test, string call)
        {
            EnsureDict();
            if (CallDict.Keys.Contains(test))
            {
                CallDict[test].Add(call);
            }
            else
            {
                CallDict.Add(test, new List<string>() { call });
            }
        }

        public static void Clear(string test)
        {
            EnsureDict();
            if (CallDict.ContainsKey(test))
            {
                CallDict[test].Clear();
            }
        }

        private static void EnsureDict()
        {
            if (CallDict == null)
            {
                CallDict = new Dictionary<string, List<string>>();
            }
        }
    }
}