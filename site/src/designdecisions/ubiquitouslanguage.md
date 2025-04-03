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

### Chain
A `Chain` referrs to a piece of a component that does processing. So `From`, `To`, `Scheduled`, `Processing` are all chains and processing is passed from one chain to the next (called because this framework is based on the design pattern `Chain Of Responsibility`).

### Component
A `Component` is what you pull in to use as part of your `Route`. A component may be something like S3 or SQS.

### Routable
`Routable` refers to the message passed between chains that is the core of the processing.

## Next Steps
Update...well...everything...eep.