# Kyameru
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

## Components
### File
#### From
The file component works as a filewatcher creating a routable event with the body data being a byte array representation of the file.
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
