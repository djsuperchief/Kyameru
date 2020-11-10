using Kyameru.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kyameru.Core.BaseComponents
{
    public class AddHeader : IProcessComponent
    {
        public event EventHandler<Log> OnLog;

        private readonly string header;
        private readonly string value;
        private readonly Func<string> callback = null;
        private readonly Func<Routable, string> callbackTwo = null;
        private readonly int callbackOption;

        public AddHeader(string header, string value)
        {
            this.header = header;
            this.value = value;
            this.callbackOption = 0;
        }

        public AddHeader(string header, Func<string> callbackOne)
        {
            this.header = header;
            this.callback = callbackOne;
            this.callbackOption = 1;
        }

        public AddHeader(string header, Func<Routable, string> callbackTwo)
        {
            this.header = header;
            this.callbackTwo = callbackTwo;
            this.callbackOption = 2;
        }

        public void Process(Routable routable)
        {
            // This is not preferred but pressed for time.
            switch (this.callbackOption)
            {
                case 0:
                    routable.AddHeader(this.header, this.value);
                    break;

                case 1:
                    routable.AddHeader(this.header, this.callback());
                    break;

                case 2:
                    routable.AddHeader(this.header, this.callbackTwo(routable));
                    break;
            }
        }
    }
}