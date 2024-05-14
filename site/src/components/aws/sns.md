---
layout: page
title: SNS
parent: AWS
grand_parent: Components
permalink: /components/aws/sns
---

# AWS SNS
## Overview

The AWS SNS component provides both a TO route allowing you to send messages to an SNS topic.


## Routes

{: .label .label-red}
FROM

{: .label .label-green }
TO

### TO

#### Setup Headers

|Header | Description | Optional | Default|
|:------ |: ----------- |: -------- |: -------|
|Host | ARN | NO | EMPTY |

#### Topic ARN

Kyameru will not validate the ARN provided to it. It assumes the ARN is correct and valid.

##### Example - ARN
```
To("sns://arn:sns:so:on:and:so:forth")
```

### Message Headers
The large majority of options for the S3 object upload are set through message headers.

|Header | Description | Optional| Default |
|:------ |: ----------- |: --------|: ----------- |
| SNSARN | ARN of SNS topic to send to | YES | Uses ARN from setup (Setting this will override the setup header) |
| SNSDeduplicationId | DeDuplication Id | YES | |
| SNSGroupId | Message group Id | YES | |

#### Other Headers

Other Kyameru headers are automatically added as message attributes and will automatically send to SNS.

### Message Body

This needs to be a string so the component can send to SNS.

## Outbound Messages
### TO

The routable message outbound from the component is as follows:

#### Body

No modifications made from inbound message.

#### Headers

|Header | Description | Immutable |
|:------ |: ----------- |: -------- |
| SNSMessageId | Id of message sent | YES |
| SNSSequenceNumber | Sequence number of the message | YES |

