using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Kyameru.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Kyameru.Component.Ses;

public class SesTo : ITo
{
    private const string CharSet = "UTF-8";
    private readonly IAmazonSimpleEmailService sesClient;
    private Dictionary<string, string> headers;

    public event EventHandler<Log> OnLog;

    public SesTo(IAmazonSimpleEmailService ses)
    {
        sesClient = ses;
    }

    public void SetHeaders(Dictionary<string, string> incomingHeaders)
    {
        headers = incomingHeaders;
    }

    public async Task ProcessAsync(Routable routable, CancellationToken cancellationToken)
    {
        Validate(routable);
        if (routable.DataType == "SesMessage")
        {
            await SendEmail(routable, cancellationToken);
        }
        else
        {
            await SendTemplate(routable, cancellationToken);
        }
    }

    private async Task SendTemplate(Routable routable, CancellationToken cancellationToken)
    {
        Log(LogLevel.Information, Resources.INFO_SENDING_TEMPLATE);
        var message = routable.Body as SesTemplate;
        var request = new SendTemplatedEmailRequest()
        {
            Source = routable.Headers.TryGetValue("SESFrom", headers["from"]),
            Destination = new Destination()
            {
                ToAddresses = routable.Headers["SESTo"].Split(",").ToList(),
                BccAddresses = GetAddresses(routable, "SESBcc"),
                CcAddresses = GetAddresses(routable, "SESCc"),
            },
            Template = message.Template,
            TemplateData = message.TemplateData
        };

        var response = await sesClient.SendTemplatedEmailAsync(request, cancellationToken);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            foreach (var key in response.ResponseMetadata?.Metadata?.Keys)
            {
                routable.SetHeader($"SES{key}", response.ResponseMetadata.Metadata[key]);
            }
        }
    }

    private async Task SendEmail(Routable routable, CancellationToken cancellationToken)
    {
        Log(LogLevel.Information, Resources.INFO_SENDING_EMAIL);
        var request = new SendEmailRequest()
        {
            Destination = new Destination()
            {
                ToAddresses = routable.Headers["SESTo"].Split(",").ToList(),
                BccAddresses = GetAddresses(routable, "SESBcc"),
                CcAddresses = GetAddresses(routable, "SESCc"),
            },
            Source = routable.Headers.TryGetValue("SESFrom", headers["from"]),
            Message = GetEmailMessage(routable)
        };

        var response = await sesClient.SendEmailAsync(request, cancellationToken);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            foreach (var key in response.ResponseMetadata?.Metadata?.Keys)
            {
                routable.SetHeader($"SES{key}", response.ResponseMetadata.Metadata[key]);
            }
        }
    }

    private Message GetEmailMessage(Routable routable)
    {
        var message = new Message()
        {
            Body = new Body()
        };
        var bodyData = routable.Body as SesMessage;
        if (!string.IsNullOrWhiteSpace(bodyData.BodyHtml))
        {
            message.Body.Html = new Content()
            {
                Charset = CharSet,
                Data = bodyData.BodyHtml
            };

        }

        if (!string.IsNullOrWhiteSpace(bodyData.BodyText))
        {
            message.Body.Text = new Content()
            {
                Charset = CharSet,
                Data = bodyData.BodyText
            };
        }

        if (!string.IsNullOrWhiteSpace(bodyData.Subject))
        {
            message.Subject = new Content()
            {
                Charset = CharSet,
                Data = bodyData.Subject
            };
        }

        return message;
    }

    private List<string> GetAddresses(Routable routable, string key)
    {
        var output = routable.Headers.TryGetValue(key);
        if (!string.IsNullOrWhiteSpace(output))
        {
            return output.Split(',').ToList();
        }

        return new List<string>();
    }

    private void Validate(Routable routable)
    {
        headers.TryGetValue("from", out var from);
        routable.Headers.TryGetValue("SESFrom", from);

        if (string.IsNullOrWhiteSpace(from))
        {
            throw new Exceptions.MissingInformationException(string.Format(Resources.EXCEPTION_MISSINGINFORMATION, "From Address"));
        }

        if (routable.Headers["DataType"] != "SesMessage" && routable.Headers["DataType"] != "SesTemplate")
        {
            throw new Exceptions.DataTypeException(Resources.EXCEPTION_INCORRECTDATATYPE);
        }

        if (routable.DataType == "SesMessage")
        {
            var message = routable.Body as SesMessage;
            if (string.IsNullOrWhiteSpace(message.BodyHtml) && string.IsNullOrWhiteSpace(message.BodyText))
            {
                throw new Exceptions.MissingInformationException(Resources.EXCEPTION_BODYMISSING);
            }
        }
        else
        {
            var message = routable.Body as SesTemplate;
            if (string.IsNullOrWhiteSpace(message.Template) || string.IsNullOrWhiteSpace(message.TemplateData))
            {
                throw new Exceptions.MissingInformationException(Resources.EXCEPTION_TEMPLATEMISSINGDATA);
            }
        }

        var to = routable.Headers.TryGetValue("SESTo");
        if (string.IsNullOrWhiteSpace(to) || to.Split(',').Count() <= 0)
        {
            throw new Exceptions.MissingInformationException(string.Format(Resources.EXCEPTION_MISSINGINFORMATION, "To Addresses"));
        }
    }

    private void Log(LogLevel logLevel, string message, Exception exception = null)
    {
        if (OnLog != null)
        {
            OnLog?.Invoke(this, new Core.Entities.Log(logLevel, message, exception));
        }
    }
}
