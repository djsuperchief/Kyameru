using Kyameru.Core.Contracts;

namespace Kyameru.Components.S3;

/// <summary>
/// Kyameru S3 To Interface
/// </summary>
public interface ITo : IToComponent
{
    void SetHeaders(Dictionary<string, string> headers);
}
