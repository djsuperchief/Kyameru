# Kyameru.Component.Slack

![Build](https://github.com/djsuperchief/Kyameru.Component.Slack/workflows/Build%20And%20Test/badge.svg)
![CodeQL](https://github.com/djsuperchief/Kyameru.Component.Slack/workflows/CodeQL/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/djsuperchief/Kyameru.Component.Slack/badge.svg?branch=main)](https://coveralls.io/github/djsuperchief/Kyameru.Component.Slack?branch=main)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Kyameru.Component.Slack)

The Slack component sends a message to a configured slack webhook.

## To
### Setup Headers

Header | Description | Optional | Default
------ | ----------- | -------- | -------
Target | Webhook in slack | NO
MessageSource | Indicates whether the message for slack should be pulled from the body of the message OR a header. | YES | Header
Channel | Specifies the channel to push the message to overriding the webhook setting | YES | EMPTY
Username | Specifies the user to appear as when pushing the message overriding the webhook | YES | EMPTY

### Message Headers

Message headers can also be used as part of the component.

Header | Description | Optional
------ | ----------- | --------
SlackMessage | If this header is specified (in conjuction with setup header) this will be the message sent to slack | YES


### Example 1: Standard Send Message

```
Kyameru.Route.From("component:///target?Arg1=value)
.Process(new Processing)
.Process<ISomeInterface>()
.To("slack:///T00000000/B00000000/XXXXXXXXXXXXXXXXXXXXXXXX?MessageSource=Body&Channel=somechannel&Username=Kyameru)
.Build(services)
```

### Example 2: Set Message In Body

```
// Assuming message is an instance of Routable

routable.SetHeader("SlackMessage", "Hello World");
```

Note that the slack target does not include the prefix of 'https://hooks.slack.com/services/'

## From
From is not supported in this component.


## More Info
For more information see Kyameru Core documentation. (https://github.com/djsuperchief/Kyameru)