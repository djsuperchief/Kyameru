---
layout: page
title: Ftp
parent: Components
---

# Overview

The Ftp component provides a very simple poller, downloader and uploader for Ftp files. It currently can only pull from a single specified directory.

## Routes

{: .label .label-green}
FROM

{: .label .label-green }
TO

### From
#### Headers

|Header | Description | Optional | Default
|:------ |: ----------- |: -------- |: -------|
|Target | Path on server | YES | /|
|Host | Ftp host | NO|
|Port | Port for the Ftp Server | YES | 21|
|PollTime | Amount of time in milliseconds to poll the server | YES | 60000|
|Filter | File filter | YES | \*.\*|
|UserName | Username to connect with | NO|
|Password | Password to connect with (can be left blank for anonymous) | YES | Empty|
|Recursive | Currently not used | YES | False |
|Delete | Indicates if a file should be deleted once downloaded | YES | False |

*Example*
```
Kyameru.Route.From("ftp://test@127.0.0.1&PollTime=5000&Filter=50000&Delete=true)
```

### To

The To route is (as expected) the route that sends a file or body data packet to the Ftp endpoint.

#### Headers

|Header | Description | Optional | Default
|:------ |: ----------- |: -------- |: -------|
|Target | Path on server | YES | /|
|Host | Ftp host | NO|
|Port | Port for the Ftp Server | YES | 21|
|PollTime | Amount of time in milliseconds to poll the server | YES | 60000|
|Filter | File filter | YES | \*.\*|
|UserName | Username to connect with | NO|
|Password | Password to connect with (can be left blank for anonymous) | YES | Empty|
|Archive | Path to archive a file once uploaded | YES | Empty|
|Source | Source of the message to upload (File or Body) | YES | File|

*Example*
```
Kyameru.Route.From([from route]).To(ftp://test@127.0.0.1/upload&Archive=../archive&Source=Body)
```

## Outbound Messages
### FROM

The routable message outbound from the component is as follows:

#### Body

The body of the message is the byte array of the downloaded file.

#### Headers

|Header | Description | Immutable |
|:------ |: ----------- |: -------- |
| SourceDirectory | Directory of downloaded file | No |
| SourceFile | File name of downloaded file | No |
| FullSource | Full path of downloaded file | No |
| DateCreated | Date and time of download | No |
| ReadOnly | Boolean indicating whether file is read only | No |
| DataType | Data type of message body (is always Byte[]) | Yes (Set by body type) |
| FtpSource | Originating URI of file | No |

### TO

No changes are made to the message.