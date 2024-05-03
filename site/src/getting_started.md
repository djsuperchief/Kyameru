---
layout: page
title: Getting Started
permalink: /gettingstarted
nav_order: 4
---

# Getting started with Kyameru

## Nuget Notes
Currently, although there is a version on NuGet, it has not been updated for a long time. The current plan is that when both the S3 and SQS components are created, the framework will be pushed to NuGet (as a beta) for general use.
Once there are enough components (not a full compliment) to make it worth while, the framework will come out of beta as V1.

## Get Your Components

As each component is reliant on the Kyameru Core library, there is no need to download that separately. Each component has been made as its own distributable so that you only need to get the components you need to use (instead of everything).
Pick the components you want to use and add them into your project.

## Creating Your First Route
### DI

As Kyameru is dependent on dependency injection (as it uses a background service and a host to run it), you will need to ensure your application host adds an appropriate host. Once this has been added, you can start building a Kyameru Route.

### From

The start of every route is from the `Kyameru.Route` namespace.

```
Kyameru.Route.From()
```

Inside the from part, enter your component URI with its settings and options / headers. For instance, I want to listen into messages on an SQS queue, so I use the AWS SQS component and add it into the from.

```
Kyameru.Route.From("sqs://myqueue")
```

Now there are more options than that (see the SQS component reference) but I have now created my entry point for the route.

### Processing
#### Add a header

This step is entirely optional but after I have pulled the message down, I want to add some additional headers. Kyameru has a built in step for adding headers so I can use that straight after I have pulled the message from SQS.

```
Kyameru.Route.From("sqs://myqueue")
.AddHeader("Test", "Test")
```

Now there are other options for headers, for instance you can use the function delegate to do some more complex processing but that's beyond the scope of this getting started document.

#### Add A Processing Component

Processing components are components you build as part of your applications domain. They add any custom logic or processing you need to the routable message before it goes through the to the TO components. You can create processing components any way you like but for the purpose of this, we'll just use the action delegate.

```
Kyameru.Route.From("sqs://myqueue")
.AddHeader("Test", "Test")
.Process((Routable x) => {
    x.SetHeader("MoreThings", "Test");
})
```
You can do what you like with the routable message and process it how you wish.


[Show Me The Source](https://github.com/djsuperchief/Kyameru){: .btn .btn-purple }