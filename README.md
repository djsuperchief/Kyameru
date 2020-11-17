# Kyameru
## About
Kyameru is a processing engine built with heavy inspiration from Apache Camel.
It is currently in very Alpha stage and contains very few components but more will be added over the coming weeks and months.

## Basic Overview
### Architecture
![Image of Kyameru Component]
(docs/arch.png)

### Workflow
![Image of workflow]
(docs/workflow.png)

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