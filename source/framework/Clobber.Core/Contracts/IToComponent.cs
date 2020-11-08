﻿using System;
using Kyameru.Core.Entities;

namespace Kyameru.Core.Contracts
{
    public interface IToComponent : IComponent
    {
        void Process(Routable item);

        void Process(Routable item, string[] args);
    }
}
