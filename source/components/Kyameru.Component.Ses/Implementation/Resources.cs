using System;

namespace Kyameru.Component.Ses;

public class Resources
{
    public const string EXCEPTION_MISSINGINFORMATION = "The SES message is missing the following information: {0}";
    public const string EXCEPTION_INCORRECTDATATYPE = "The body of the routable message is not of type SesMessage";
    public const string EXCEPTION_BODYMISSING = "You must provide either body html or body text.";
    public const string EXCEPTION_TEMPLATEMISSINGDATA = "You must provide both the template name and template data.";

    public const string INFO_SENDING_EMAIL = "Sending email without template data.";

    public const string INFO_SENDING_TEMPLATE = "Sending email using template data";
}
