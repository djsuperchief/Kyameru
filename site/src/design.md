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
2. RouteBuilder
3. Builder


`Route` is the entry point to create your Kyameru processing. It allows you to specify your starting chain link from your downloaded components and then construct a `Route` that consists of `To` chain links as well as a number of other `Processors`. The `From` chain link starts the process and then each part of the Route, in turn, passes data through to the next processor in the chain until the last chain link has executed.

A `To` chain link is the final step in the chain (apart from `Error` which deals with errors....obviously) and you can have multiple `To` chain links for example having a final step to write a file to physical storage as well as cloud storage.
None of the components you use are aware of each other and each chain link executes in isolation. The only link between them is the `Routable` data passed between each chain link and component.

### Workflow
![Image of workflow](/assets/img/workflow.png)

The workflow is fairly simple. Everything starts at the From chain link. This raises an OnAction event which is picked up by the From chain object that then starts the process off. The Routable message is then passed from chain link to chain link (or processor) until it reaches the last `To` chain link.
What is important is each of the declared chain links sit within a core Chain object responsible for handling logging, errors and passing execution to injected entities during the build.

If at any point the Routable message is deemed to be in error, all processing stops and the only chain link that continues to process is an Error processor that is entirely optional.

## More Information

To view more on the project, go to the [GitHub source](https://github.com/djsuperchief/Kyameru).