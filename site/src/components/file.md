---
layout: page
title: File
parent: Components
---

# Overview

The file component provides both FROM and TO routes to allow you to watch a folder for changes and also to write a file to a location.

## Routes
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