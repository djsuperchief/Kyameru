using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Internal;
using Amazon.SimpleEmail.Model;
using Kyameru.Component.Ses.Exceptions;
using Kyameru.Core.Entities;
using NSubstitute;

namespace Kyameru.Component.Ses.Tests;

public class SesToTests
{
    [Fact]
    public async Task CanSendStandardMessageWithComponentFrom()
    {
        var messageId = Guid.NewGuid();
        var to = new SesTo(GetMockedClient(messageId));
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "from", "from@test.com" }
        });
        var routable = new Routable(new Dictionary<string, string>()
        {
            { "SESTo", "test@test.com" },
        }, "Any data");
        routable.SetBody<SesMessage>(new SesMessage()
        {
            BodyHtml = "<h1>Hello</h1>",
            BodyText = "Text of body",
            Subject = "Subject line"
        });
        await to.ProcessAsync(routable, default);
        Assert.Equal(messageId.ToString(), messageId.ToString());
        Assert.Equal("from@test.com", routable.Headers["SESTest-From"]);
        Assert.Equal("test@test.com", routable.Headers["SESTest-To"]);
    }

    [Fact]
    public async Task IncorrectDataTypeThrowsError()
    {
        var messageId = Guid.NewGuid();
        var to = new SesTo(GetMockedClient(messageId));
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "from", "from@test.com" }
        });
        var routable = new Routable(new Dictionary<string, string>()
        {
            { "SESTo", "test@test.com" },
        }, "Any data");

        routable.SetBody<string>("test");
        await Assert.ThrowsAsync<DataTypeException>(() => to.ProcessAsync(routable, default));
    }

    [Fact]
    public async Task BlankToAddressThrowsError()
    {
        var messageId = Guid.NewGuid();
        var to = new SesTo(GetMockedClient(messageId));
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "from", "from@test.com" }
        });
        var routable = new Routable(new Dictionary<string, string>()
        { }, "Any data");
        routable.SetBody<SesMessage>(new SesMessage()
        {
            BodyHtml = "<h1>Hello</h1>",
            BodyText = "Text of body",
            Subject = "Subject line"
        });

        await Assert.ThrowsAsync<MissingInformationException>(() => to.ProcessAsync(routable, default));
    }

    [Fact]
    public async Task BlankDataThrowsError()
    {
        var messageId = Guid.NewGuid();
        var to = new SesTo(GetMockedClient(messageId));
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "from", "from@test.com" }
        });
        var routable = new Routable(new Dictionary<string, string>()
        {
            { "SESTo", "test@test.com" },
        }, "Any data");
        routable.SetBody<SesMessage>(new SesMessage()
        {
            Subject = "Subject line"
        });

        await Assert.ThrowsAsync<MissingInformationException>(() => to.ProcessAsync(routable, default));
    }

    [Fact]
    public async Task TemplateSendsOk()
    {
        var messageId = Guid.NewGuid();
        var to = new SesTo(GetMockedClient(messageId));
        to.SetHeaders(new Dictionary<string, string>()
        {
            { "from", "from@test.com" }
        });
        var routable = new Routable(new Dictionary<string, string>()
        {
            { "SESTo", "test@test.com" },
        }, "Any data");
        routable.SetBody<SesTemplate>(new SesTemplate()
        {
            Template = "Test",
            TemplateData = "{\"test\":\"test\"}"
        });

        await to.ProcessAsync(routable, default);
        Assert.Equal(messageId.ToString(), messageId.ToString());
        Assert.Equal("from@test.com", routable.Headers["SESTest-From"]);
        Assert.Equal("test@test.com", routable.Headers["SESTest-To"]);
    }

    private IAmazonSimpleEmailService GetMockedClient(Guid? messageId = null)
    {
        messageId ??= Guid.NewGuid();
        var sesClient = Substitute.For<IAmazonSimpleEmailService>();

        sesClient.SendEmailAsync(Arg.Any<SendEmailRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var response = new SendEmailResponse()
            {
                MessageId = messageId.ToString()
            };
            var request = x[0] as SendEmailRequest;
            response.HttpStatusCode = System.Net.HttpStatusCode.OK;
            response.ResponseMetadata = new Amazon.Runtime.ResponseMetadata();
            response.ResponseMetadata.Metadata.Add("Test-To", string.Join(",", request.Destination.ToAddresses));
            response.ResponseMetadata.Metadata.Add("Test-From", string.Join(",", request.Source));


            return response;
        });

        sesClient.SendTemplatedEmailAsync(Arg.Any<SendTemplatedEmailRequest>(), Arg.Any<CancellationToken>()).Returns(x =>
        {
            var response = new SendTemplatedEmailResponse()
            {
                MessageId = messageId.ToString()
            };
            var request = x[0] as SendTemplatedEmailRequest;
            response.HttpStatusCode = System.Net.HttpStatusCode.OK;
            response.ResponseMetadata = new Amazon.Runtime.ResponseMetadata();
            response.ResponseMetadata.Metadata.Add("Test-To", string.Join(",", request.Destination.ToAddresses));
            response.ResponseMetadata.Metadata.Add("Test-From", string.Join(",", request.Source));

            return response;
        });

        return sesClient;
    }
}
