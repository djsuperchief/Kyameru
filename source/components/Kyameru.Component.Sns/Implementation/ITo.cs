﻿using Kyameru.Core.Contracts;

namespace Kyameru.Component.Sns;

public interface ITo : IToComponent
{
    void SetHeaders(Dictionary<string, string> headers);
}
