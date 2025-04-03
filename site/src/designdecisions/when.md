---
layout: page
title: When
parent: Design Decisions
---

# When
## Problem Statement
As a user, I want to be able to execute a `To` chain but only based on certain conditions.

## Considerations
So this seems like a perfectly valid use case. If a user wants to only do something based on the output of processing / contents of a routable message then it makes more sense to allow this in a single route rather than create separate routes to deal with different branches of logic on the same item.
A user should be able to deal with conditional processing inside a processing chain BUT once you get to the final `To` chains, there is nothing to allow you to send to other chains conditionally (at least not easily).

What is needed is some rudimentary conditional flow that allows you to perform some basic logic on the `Routable` item and then execute a `To` chain based on wther the result is true. I don't think right now that an else is required but that will come out of real life usage.

## Solution Choice
Well there isn't a lot to choose from here. A `When` is essentially a `To` chain but with some additional processing to establish whether it should execute or not. Every To chain could potentially be conditional so adding a property to the `RouteAttributes` object seems the most logical option.
Also, making it a Func will limit the amount of potential implementations required and the choice is supposed to be simple.

### Config
Ah, so with config, we will need to do this via reflection. The reason for this is that I don't want to introduce a vulnerability that will allow arbitrary C# code to be executed after being injected from config file. Only config based routes will require this (or it's what I am mandating anyway) so we should consider making this an internal method.