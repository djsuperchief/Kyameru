using Kyameru.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Component.Slack
{
    public class Inflator : IOasis
    {
        public IFromComponent CreateFromComponent(string[] args)
        {
            throw new NotImplementedException();
        }

        public IFromComponent CreateFromComponent(Dictionary<string, string> headers)
        {
            throw new NotImplementedException();
        }

        public IToComponent CreateToComponent(string[] args)
        {
            throw new NotImplementedException();
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers)
        {
            return new SlackTo(headers);
        }
    }
}