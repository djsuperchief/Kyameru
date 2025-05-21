---
layout: page
title: Ubiquitous Language
parent: Design Decisions
---

# Ubiquitous Language
## What Is It
Essentially it is the language used by everyone in a project from stakeholders down to engineering to improve communication and reduce misunderstandings.

## So...how / why does it apply here
One thing I have noted is that I have tended to chop and change the terms I use through documentation and code which is bound to make things confusing. Whilst I completely understand what is used where and when, this doesn't make it any easier for someone trying to implement this framework in an application.
I need to be more consistent through code and through documentation to make sure everyone is clear how it works.

## Definitions
### Kyameru
`Kyameru` is the name of the framework.

### Route
`Route` is the name of what you create to take a message from a starting point to its eventual end point. A `Route` consists of Components, chains and a Routable message.

### Component
A `Component` is what you pull in to use as part of your `Route`. A component may be something like S3 or SQS.

### Chain Link
A `Chain Link` refers to the implementation of the `From` or `To` parts of a component. So a component contains `Chain Links` that in turn are consumed by `Chains` in the core library.

### Chain
A `Chain` is the fundemental implementation of the Chain Of Responsibility pattern. Each chain passes execution through to the next (depending on your `Route` setup) until the last in the chain.

### Processor
A `Processor` refers to any object or class acting directly on a `Routable` message. Even though the `Process` part of the a Route is executed in a chain, because it is optional it is more of a processor than a `Chain Link`.

### Routable
The `Routable` is the data passed from one `Chain` to another. This data is updated as it goes through each `Chain` and `Processor`.