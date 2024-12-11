---
layout: page
title: Getting Started
permalink: /gettingstarted
nav_order: 4
---

# Getting started with Kyameru

## Get Your Components

As each component is reliant on the Kyameru Core library, there is no need to download that separately. Each component has been made as its own distributable so that you only need to get the components you need to use (instead of everything).
Select the components you want from NuGet (pre-release for the latest development versions) and...that's it.

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

Processing components are components you build as part of your applications domain. They add any custom logic or processing you need to the routable message before it goes through the to the TO components. You can create processing components in many ways:

- Concrete implementation
- Dependency Injection
- Reflection (should only really use with config)
- Action
- Async Function

But for the purpose of this, we'll just use the action delegate.

```
Kyameru.Route.From("sqs://myqueue")
.AddHeader("Test", "Test")
.Process((Routable x) => {
    x.SetHeader("MoreThings", "Test");
})
```
You can do what you like with the routable message and process it how you wish.

### To Route

The To route works in the same way as the from. You specify what component you want to route to (with any additional setup headers) and that's it!
The To route does have some additional parts to it.

#### Multiple To Routes

You can continue to chain To components together so if you wanted to say put a file in an S3 bucket, sFTP and then archive it, you would chain these three To components together.

#### To Post Processing

Every To component has the ability to add post processing to it. This post processing is a `processing component` that you create (the same as an ordinary Processing Component) and it is executed immediately after the To component has finished.

```
.To("component://setup", new MyComponent())
.To<IMyComponent>("component://setup")
.To("component://setup", "MyNamespace.Component")
.To("Component://setup", (Routable x) => {})
.To("Component://setup", async (Routable x) => {})
```

### Id

Every route can be assigned an Id specified by you or it will be assigned at random. To specify an Id, use the Id function.

```
.Id("my-route")
```

This may help identify errors if you use several routes that are all the same.

### Error Route

You can create an error processing component `IErrorComponent` that execute if the route encounters any errors. This gives you an opportunity to do any final processing on a message in the event the route encounters any errors.

### Scheduling

You can schedule a route to execute every (x) minutes, hours or at (x) hour or minute. Full Cron support is not available right now but it will be in future iterations.

```
.ScheduleEvery(Core.Enums.TimeUnit.Minute, 1)

OR

.ScheduleAt(Core.Enums.TimeUnit.Minute, 1)
```

It is important to note that whilst you can ask for something to be scheduled, a component has to be able to support it (FROM part of the component) so ensure you are using a component that can support a schedule.

### Full Example

```
Kyameru.Route.From("sqs://myqueue")
.AddHeader("Test", "Test")
.Process((Routable x) => {
    x.SetHeader("MoreThings", "Test");
})
.Id("my-route")
.To("s3://mybucket/path)
.Build(services);
```

## Summary

And thats it! When your host application starts, the hosted service will start your from route and you can leave Kyameru to it.
To see all the currently available components, go to the [Components](components) section.

## Source

[Show Me The Source](https://github.com/djsuperchief/Kyameru){: .btn .btn-purple }