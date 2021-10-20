# Kyameru.Component.Ftp

![Build](https://github.com/djsuperchief/Kyameru.Component.Ftp/workflows/Build/badge.svg)
![CodeQL](https://github.com/djsuperchief/Kyameru.Component.Ftp/workflows/CodeQL/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Kyameru.Component.Ftp)

The Ftp component provides a very simple poller, downloader and uploader for Ftp files. It currently can only pull from a single specified directory.

*Works With*
* Kyameru Core (https://github.com/djsuperchief/Kyameru)
* Kyameru File Component (https://github.com/djsuperchief/Kyameru.Component.File)

## From
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

## To

The To route is (as expected) the route that sends a file or body data packet to the Ftp endpoint.

Header | Description | Optional | Default
------ | ----------- | -------- | -------
Target | Path on server | YES | /
Host | Ftp host | NO
Port | Port for the Ftp Server | YES | 21
PollTime | Amount of time in milliseconds to poll the server | YES | 60000
Filter | File filter | YES | \*.\*
UserName | Username to connect with | NO
Password | Password to connect with (can be left blank for anonymous) | YES | Empty
Archive | Path to archive a file once uploaded | YES | Empty
Source | Source of the message to upload (File or Body) | YES | File

*Example*
```
Kyameru.Route.From([from route]).To(ftp://test@127.0.0.1/upload&Archive=../archive&Source=Body)
```