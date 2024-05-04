---
layout: page
title: General Principals
permalink: /principals
nav_order: 3
---

# General Principals

To make it as easy as possible to create components, a URI format has been used as it is likely to be known to most developers. It also makes it easier to specify it at run time and allow for the framework to run.

## Background Service

Kyameru is built as a background service and each route created is a separate background service. It is designed this way to ensure that it doesn't impact foreground processing.

## Dependency Injection

Kyameru is also built to utilize dependency injection. This allows you to be able to configure components such as AWS in your own way by injecting the config in your application and then allowing Kyameru to do the rest. It was designed this way because it didn't seem productive to re-invent the wheel when it came to specifying config.

Processing components that you create can also be injected through DI as well as being created inline and as class instances.

## Processing Order

It goes without saying that the order that components are registered in is the order they will be executed in. Once registered, this order cannot be changed. This order is guaranteed.

## "Routable" message
### Overview

Data is passed through to each component by way of a "Routable" message. This message has a body and headers that can be modified during its lifetime.

### Headers

A routable message contains headers that get added by either your own processing or as part of a component. Some headers are editable and others will be immutable. For instance, the `File From` component will add an immutable header for where it picked the file up from. Once it has been added, it cannot be modified or removed.

### Immutable Headers

Although most parts of the routable message are designed to be mutable, each component may specify that some headers created by it will be immutable. This is to protect certain aspects of the original message through the framework. For example, the file component specifies the `FullSource` header as immutable to protect the originating source file.

### Body

The body of the routable message can be anything. It is advised to stick to byte array or a string but essentially, so long as a component can "read" the body, it can be anything.

## Async

Kyameru runs asynchronously (as it is hosted in an `IHostedService`) so all operations (including process components) run as such.

## URI Syntax

As specified before, the components are instantiated using the URI format. The query part of the URI is always used for headers but the host and path may sometimes be used as a single entity known as `target`.

```
component://host/path?header=value
```

For example:

```
s3://mybucket/path
```
This states an AWS S3 component specifying the bucket name `mybucket` and the path in the bucket as `path`.


```
file:///c/test?Notifications=Created
```

This specifies the file component pointing to the folder `/c/test` and that it will only trigger on new files created.

{: .note}
> Some components may have a tweak to the URI syntax due to how it is parsed but each component will document this if applicable. Where possible, Kyameru will follow current principals.

## Home Made Components

It is entirely possible to build your own Kyameru components. If you wish to do so, you will need to make sure your component has the namespace `Kyameru.Component`:

{: .warning }
We do not offer support for home grown components but some questions or queries may be answered in the issues section of the GitHub project.