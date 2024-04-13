---
layout: page
title: File
parent: Components
---

# Overview

The file component provides both FROM and TO routes to allow you to watch a folder for changes and also to write a file to a location.

## Routes

{: .label .label-green}
FROM

{: .label .label-green }
TO

### FROM

#### Headers

| Header | Description | Optional |
|:------ |: ----------- |: --------|
|Target | Folder to watch | NO |
|Notifications | Type of notification to raise | NO |
|Filter | File watch filter | NO |
|SubDirectories | Include sub directories | YES |
|InitialScan | Whether it should scan the target folder before watching | YES |
|Ignore | Directories to ignore (separated by pipes) | YES |
|IgnoreStrings | Strings in file names to ignore (separated by pipes) | YES |

#### URI Parts

| URI Part | Description |
|: ------- |: ---------- |
| Host | Not used |
| Path | Full path of folder |

#### Example Syntax
```
Kyameru.Route.From("file:///C:/Temp?Notifications=Created&SubDirectories=true&Filter=*.*")
```

#### Message Headers Raised

| Header | Description | Immutable |
|:------ |: ----------- |: --------- |
|SourceDirectory | Directory the event is raised from | YES |
|SourceFile | File name of file picked up | NO |
|FullSource | Full path of the file picked up | YES |
|DateCreated | Date and time of the file (UTC) | YES |
|Readonly | Boolean as to whether the file is readonly | NO |
|Method | How the file was picked up | NO |
|DataType | The data type of the body | NO |

### To

The to component does a couple of very simple actions:

* Moves the picked up source file
* Copies the picked up source file
* Deletes the picked up source file
* Writes the contents of the body of the message to a file with the same name in the destination directory

{: .note }
> The deletes picked up source file is currently under review.

#### Setup Headers

Header | Description | Optional
------ | ----------- | --------
Target | Destination Directory | NO
Action | Action To Perform | NO
Overwrite | Overwrites the destination if it exists | YES

*Example Syntax*
```
.To("file:///C:/Temp?Action=Move&Overwrite=true")
```

#### Source File
The target is the root folder that it will put files into but it is important to note that you can specify a sub directory of a file by ensuring that the SourceFile header contains the sub directory and file name.

```
routable.SetHeader("SourceFile", "SubDirectory/filename.txt");
```
