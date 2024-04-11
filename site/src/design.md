---
layout: page
title: Design Overview
permalink: /overview
nav_order: 2
---

## Basic Design Overview
### Architecture

![Image of Kyameru Component](/assets/img/arch.png)

### Structure
Kyameru is built on the Chain Of Responsibility pattern. It is built up using the three main construction objects:

1. Route
1. RouteBuilder
1. Builder

Each of the objects above are instantiated in the order specified. Route provides the start of the construction by allowing the definition of a From component. This returns a RouteBuilder object allowing you to add Processing components, add headers or a number of other tasks related to message processing.
The To component is also created here which in turn returns the final Builder object which allows you to add more To components for final processing and an Error component to handle any chain errors.

The Builder component is responsible for injection into the DI container of .NET Core (which is in turn responsible for resolving ILogger instances) and creates an IHosted service for what is known as a Route.

### Workflow
![Image of workflow](/assets/img/workflow.png)

The workflow is fairly simple. Everything starts at the From component. This raises an OnAction event which is picked up by the From chain object that then starts the process off. The Routable message is then passed from component to component until it reaches the last To component in the chain.
What is important is each of the declared components sit within a core Chain object responsible for handling logging, errors and passing execution to injected components during the build.

If at any point the Routable message is deemed to be in error, all processing stops and the only component that continues to process is an Error component that is entirely optional.

## More Information

To view more on the project, go to the [GitHub source](https://github.com/djsuperchief/Kyameru).