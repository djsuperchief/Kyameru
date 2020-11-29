# Kyameru
![Build Status](https://github.com/djsuperchief/Kyameru/workflows/.NET%3ACore/badge.svg)
## About
Kyameru is a processing engine built with heavy inspiration from Apache Camel.
It is currently in very Alpha stage and contains very few components but more will be added over the coming weeks and months.

## Basic Overview
### Architecture
![Image of Kyameru Component](docs/arch.png)
#### Structure
Kyameru is built on the Chain Of Responsibility pattern. It is built up using the three main construction objects:

1. Route
1. RouteBuilder
1. Builder

Each of the objects above are instantiated in the order specified. Route provides the start of the construction by allowing the definition of a From component. This returns a RouteBuilder object allowing you to add Processing components, add headers or a number of other tasks related to message processing.
The To component is also created here which in turn returns the final Builder object which allows you to add more To components for final processing and an Error component to handle any chain errors.

The Builder component is responsible for injection into the DI container of .NET Core (which is in turn responsible for resolving ILogger instances) and creates an IHosted service for what is known as a Route.

### Workflow
![Image of workflow](docs/workflow.png)

The workflow is fairly simple. Everything starts at the From component. This raises an OnAction event which is picked up by the From chain object that then starts the process off. The Routable message is then passed from component to component until it reaches the last To component in the chain.
What is important is each of the declared components sit within a core Chain object responsible for handling logging, errors and passing execution to injected components during the build.

If at any point the Routable message is deemed to be in error, all processing stops and the only component that continues to process is an Error component that is entirely optional.

### Atomicity
#### Atomic Option
Currently under construction, each route can be configured to be atomic. The From component is responsible for the atmic nature of the route for insance if you use the Ftp component and specify that it needs to be atomic, it will not delete the downloaded file from the Ftp server until the To component executes successfully.

#### Current Atomicity.
Each component is considered atomic as opposed to the entire route. The engineer is expected to be able to handle failures through the use of the Error component to ensure the route is atomic. The reason for this is that a From component cannot assume what the end To component will do and as such cannot assume what action needs to be taken.

#### FTP Example
For instance an FTP download will download a file to a temporary location to be processed. Whilst it can assume some sort of processing on the data of that file will happen it is not known what the end state of the file will be. The FTP component will clean up after itself but it is up to the engineer to decide if any processing failed on the file, what should they do with the temporary file downloaded? Should they move it to an error folder? As far as the FTP component is concerned, its job was to download the file, delete the source and raise the internal message for processing. Its process has been atomic and it now plays no further part in the processing of the file.

## Basic Syntax
### URI Format

As stated before, Kyameru is inspired by Apache Camel and the URI format of injecting components seemed like a sensible approach as it is highly descriptive and a format we have been using for decades. The Uri is split into several parts:

* Scheme = Kyameru component
* Path = Kyameru "target" header
* Query = Kyameru headers

An example of this (file component)

```
Kyameru.Route.From("file:///C:/Test?Notifications=Created&SubDirectories=true&Filter=*.*)
.To("file:///C:/backup?Action=Move)
.Build(services);
```

The above example is very simple but you can see from the syntax that the From construction that:

* Scheme is file -> this intializes the from component Kyameru.Component.File
* Path is C:/Test -> this sets the component header "Target" to C:/Test
* Query -> Adds the headers Notifications, SubDirectories and Filter

## Components
### File (https://github.com/djsuperchief/Kyameru.Component.File)
The File component can watch a directory for files and raise a message through a route to indicate it has found a new file. It can also move files from one place to another either by writing the contents of the Routable message OR from disk.

#### From

The from component is a simple system file watcher raising notifications for when a file has been:

* Added
* Modified
* Renamed

##### Setup Headers

Header | Description | Optional
------ | ----------- | --------
Target | Folder to watch | NO
Notifications | Type of notification to raise | NO
Filter | File watch filter | NO
SubDirectories | Include sub directories | YES

*Example Syntax*
```
Kyameru.Route.From("file:///C:/Temp?Notifications=Created&SubDirectories=true&Filter=*.*")
```

##### Message Headers Raised
Header | Description
------ | -----------
SourceDirectory | Directory the event is raised from
SourceFile | File name of file picked up
FullSource | Full path of the file picked up
DateCreated | Date and time of the file (UTC)
Readonly | Boolean as to whether the file is readonly
Method | How the file was picked up
DataType | The data type of the body

#### To

The to component does a couple of very simple actions:

* Moves the picked up source file
* Copies the picked up source file
* Deletes the picked up source file
* Writes the contents of the body of the message to a file with the same name in the destination directory

##### Setup Headers

Header | Description | Optional
------ | ----------- | --------
Target | Destination Directory | NO
Action | Action To Perform | NO
Overwrite | Overwrites the destination if it exists | YES

### Slack (https://github.com/djsuperchief/Kyameru.Component.Slack)
The Slack component sends a message to a configured slack webhook.
#### To
##### Setup Headers

Header | Description | Optional | Default
------ | ----------- | -------- | -------
Target | Webhook in slack | NO
MessageSource | Indicates whether the message for slack should be pulled from the body of the message OR a header. | YES | Header

### Ftp (https://github.com/djsuperchief/Kyameru.Component.Ftp)
The Ftp component provides a very simple poller, downloader and uploader for Ftp files. It currently can only pull from a single specified directory.

#### From
Header | Description | Optional | Default
------ | ----------- | -------- | -------
Target | Path on server | YES | /
Host | Ftp host | NO
Port | Port for the Ftp Server | YES | 21
PollTime | Amount of time in milliseconds to poll the server | YES | 60000
Filter | File filter | YES | \*.\*
UserName | Username to connect with | NO
Password | Password to connect with (can be left blank for anonymous) | YES | Empty
Recursive | Currently not used | YES | False
Delete | Indicates if a file should be deleted once downloaded | YES | False

*Example*
```
Kyameru.Route.From("ftp://test@127.0.0.1&PollTime=5000&Filter=50000&Delete=true)
```

## Current Status
This project is still very much in beta but has been released to the wider community early for feedback and to be used.