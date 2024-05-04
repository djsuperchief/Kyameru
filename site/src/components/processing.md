---
layout: page
parent: Components
nav_order: 1
title: Processing Component
---

# Overview

The processing component is the part designed for you to build. It is the part of the route that you decide what extra logic to apply to the routable message. For instance, if you pick up a file using the File component, then you could develop a processing component to read the file, apply some headers to the message and then write out to SQS.
The processing components are for you to write any way you like.

# Component order

The order in which you add your processing components is the order in which they will be executed. Once it has been defined in the route, it cannot be changed.

# Execution Type

There are five ways in which to add a processing component:

- Concrete type
- Dependency Injection
- Type Name (reflection)
- Action delegate
- Async function delegate

## General Rules

Any processing component class you create must inherit from the `IProcessComponent` interface in order to be correctly added to the route.

## Concrete Type

To add a concrete type to the chain, you can do the following:

```
Route.From()
.Process(new MyConreteType())
```

## Dependency Injection

To add a DI based component:

```
Route.From()
.Process<IMyComponent>()
```

## Type Name

{: .warning}
> It is advised to not use this method as this is provided to allow config from json to work

To add a type name based component:

```
Route.From()
.Process("My.Namespace.Component.Class")
```

## Action Delegate

To add an action delegate:

```
Route.From()
.Process((Routable x) => {
    // do some processing
})
```

## Async Function Delegate

To add an Async function delegate:

```
Route.From()
.Process(async (Routable x) => {
    // do some processing
    await Task.CompletedTask();
})
```