﻿using System;
using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.File
{
    public class Inflator : IOasis
    {
        public IFromComponent CreateFromComponent(string[] args)
        {
            return new FileWatcher(args);
        }

        public IFromComponent CreateFromComponent(Dictionary<string, string> headers)
        {
            return new FileWatcher(headers);
        }

        public IToComponent CreateToComponent(string[] args)
        {
            return new FileTo(args);
        }

        public IToComponent CreateToComponent(Dictionary<string, string> headers)
        {
            return new FileTo(headers);
        }
    }
}