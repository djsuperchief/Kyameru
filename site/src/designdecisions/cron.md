---
layout: page
title: Cron
parent: Design Decisions
---

# Cron
## Motivation

A user may wish to schedule routes to execute at a particular time or interval and outside of building their own scheduling job or importing a scheduling framework, there is nothing available in Kyameru.
The other motivation is that a number of components do not provide a From chain because there is no legitimate trigger for it. For instance SQL, whilst in certain providers you will be able to watch for events on a table (such as inserts, deletes etc), this is not something central to every SQL provider. As such, there is no From chain available. Adding a Cron will enable a From chain to be created and enable a From chain in components where an execution trigger would otherwise not be possible.

## Considerations

### Cron Syntax

There is no benefit in creating an specialized Cron syntax and Cron is a well known format. Standard Cron syntax should be adopted to make it as easy to configure as possible.

### Route Isolation

An early design decision was that each Route should be isolated from any other and operate in it's own space. Therefor each Route was implemented as its own HostedService. Any design for Cron should not break this design principle.

## Solution Choices

There are various choices on how to implement a Cron and how it should be implemented and the options are below.

### Existing framework (E.G HangFire)

Scheduling is a well trodden path and many implementations have already been created that will most likely do a better job than anything we create. The biggest problem here is that we will be adding a significant dependency to the project regardless of whether they want it or not. There is an option to have the Cron as a separate project and just extend the RouteBuilder etc and only include it if you want to be able to use it.
Using an existing framework ultimately removes the burden of managing any jobs away from Kyameru itself and pushes the development of this functionality forwards much faster than anticipated.

### Central Cron Scheduler

Don't over complicate it. A route needs to trigger at a specific time which is calculated from a Cron. We know when it needs to be executed.
If we do not go down the route of bringing in a central framework, we have to develop something ourselves. Whether this is an built in component in the core or something that is optionally installed is more of an implementation decision but having a core mechanism means that we need to have all jobs "register" and then the central Cron can execute each route specified by the Cron.
This design (as well as using an existing framework) break the core design principle of each Route is isolated. Although the route itself will be isolated from another, they are coupled by a central Cron processing component. If this component fails, all Routes fail.
Another consideration is that a central component will also be responsible for the concurrent execution of each Route along with its management, cancellation etc. This is a huge burden to add to the framework and not a small undertaking. Not only that, this may not be a widely used function and it could be a significant effort for very little gain.

### Cron Scheduler Pub / Sub

Very similar to the previous option except that instead of the scheduler being responsible for the execution of each Route, it is only responsible for issuing "Route Start" events when a Routes Cron time has been reached.
The advantage of this is that each route remains isolated and its concurrent execution is managed by the NETCore framework. However, we still loosely couple each route to a central mechanism that, if it fails, every route coupled to it will now by definition, have failed.
Moreover, it means potentially pulling in another Pub / Sub framework to manage this functionality (which again could be mitigated by creating it as a separate package).
This option is more manageable and future proof than the previous but again breaks a design principle.

### Component Cron

This option obeys the design principle of Route Isolation in that each component is responsible for managing its own Cron (all be it, the core does the actual implementation of the Cron but it is isolated to each Route). This allows us to greatly simplify the implementation of it as each Component starts from a custom From chain (there is already a base one in the core) and this simply knows when it should execute. How this is done is an implementation detail but it means that if this components Cron fails or falls over, others are not affected.
The burden of execution falls to the framework and each Route continues to be isolated.

## Decision

`Component Cron` is the option that has the most benefit and the least disadvantages. It is certainly worth exploring first before looking at the other options. The key to any of this is that the builder syntax needs to built in a way that if the underlying mechanism changes, uses of it do not need to be rebuilt (I.E no breaking changes).