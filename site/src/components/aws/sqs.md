---
layout: page
title: SQS
parent: AWS
grand_parent: Components
permalink: /components/aws/sqs
---

# AWS SQS
## Overview

The AWS SQS component provides both a TO and FROM route allowing you to send and receive messages via SQS.


## Routes

{: .label .label-green}
FROM

{: .label .label-green }
TO

### FROM

|Header | Description | Optional | Default|
|:------ |: ----------- |: -------- |: -------|
|Host | Queue name or URL | NO| EMPTY |
|PollTime | The time specified in seconds between polls at the specified queue | YES | 10 |
|http | Boolean value indicating if http should be used instead of https | YES | false |

#### Queue URL or Name

When specifying the URL, it is important to exclude the https part of the URL. Kyameru will append this automatically for you.

#### Example - Queue Name

```
Kyameru.Route.From("sqs://myqueue?PollTime=5")...
```

#### Example - QueueUrl (using LocalStack as an example)
```
Kyameru.Route.From("sqs://localhost:4566/000000000000/kyameru-from?PollTime=5")...
```

#### Delete Messages / Visibility Timeout

The from component will automatically delete messages from the SQS queue. In terms of visibility timeout, the component makes no changes to what has been specified in the SQS setup so it is imperative that this be setup correctly.

### TO

#### Setup Headers

|Header | Description | Optional | Default|
|:------ |: ----------- |: -------- |: -------|
|Host | Queue name or URL | YES | EMPTY |

#### Queue URL or Name

The same rules apply as the from route,.

##### Example - Queue Name
```
To("sqs://myqueue")
```

##### Example - Queue URL

```
To("sqs://sqs://localhost:4566/000000000000/kyameru-to")
```

### Message Headers
The large majority of options for the S3 object upload are set through message headers.

|Header | Description | Optional| Default |
|:------ |: ----------- |: --------|: ----------- |
| SQSQueue | SQS Queue to send to | YES | Uses queue from setup |

## Outbound Messages
### FROM

The routable message outbound from the component is as follows:

#### Body

SQS Message Body

#### Headers

`SQSMessageId` is the only header added and all message attributes are also added as headers.

### TO

The routable message outbound from the component is as follows:

#### Body

No modifications made from inbound message.

#### Headers

|Header | Description | Immutable |
|:------ |: ----------- |: -------- |
| SQSMessageId | Id of message sent | Yes |


### Message Notes
#### Queue - TO Only

The queue can also be specified as part of the message headers. If it is specified in the message then this will take precedence over the configured queue during the setup.

#### All Headers

All headers in the message will be converted to a message attribute when sending an SQS message. On the receiving side, all message attributes will be converted to a message header.