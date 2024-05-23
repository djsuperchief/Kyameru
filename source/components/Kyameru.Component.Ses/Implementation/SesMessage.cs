using System;

namespace Kyameru.Component.Ses;

/// <summary>
/// Message for Ses.
/// </summary>
/// <remarks>
/// This is for the body and subject only. Addresses are still expected in headers.
/// </remarks>
public class SesMessage
{
    public string BodyText { get; set; }

    public string BodyHtml { get; set; }

    public string Subject { get; set; }
}
