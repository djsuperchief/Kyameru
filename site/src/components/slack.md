---
layout: page
title: Slack
parent: Components
---

# Overview

The Slack component sends a message to a configured Slack webhook.

{: .note}
> Webhooks in Slack are still supported but this may be removed in the future. It is also not the advised way to send messages into Slack but this component is being provided as a legacy support avenue.

## Routes

{: .label .label-red}
FROM

{: .label .label-green }
TO

### To
#### Setup Headers

|Header | Description | Optional | Default|
|:------ |: ----------- |: -------- |: -------|
|Target | Webhook in Slack | NO|
|MessageSource | Indicates whether the message for Slack should be pulled from the body of the message OR a header. | YES | Header |
|Channel | Specifies the channel to push the message to overriding the webhook setting | YES | EMPTY|
|Username | Specifies the user to appear as when pushing the message overriding the webhook | YES | EMPTY|

#### Message Headers

Message headers can also be used as part of the component.

|Header | Description | Optional|
|:------ |: ----------- |: --------|
|SlackMessage | If this header is specified (in conjuction with setup header) this will be the message sent to Slack | YES|


### Example 1: Standard Setup

```
Kyameru.Route.From("component:///target?Arg1=value)
.Process(new Processing)
.Process<ISomeInterface>()
.To("slack:///T00000000/B00000000/XXXXXXXXXXXXXXXXXXXXXXXX?MessageSource=Body&Channel=somechannel&Username=Kyameru)
.Build(services)
```

### Example 2: Set Message In Header

```
// Assuming message is an instance of Routable

routable.SetHeader("SlackMessage", "Hello World");
```

### Example 3: Set Message In Body
```
routable.SetBody<string>("Hello World");
```

Note that the Slack target does not include the prefix of 'https://hooks.slack.com/services/'