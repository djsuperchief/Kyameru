using System.Collections.Generic;
using Kyameru.Core.Contracts;

namespace Kyameru.Component.Sqs;

public interface IFrom : IFromComponent
{
    void SetHeaders(Dictionary<string, string> headers);
}