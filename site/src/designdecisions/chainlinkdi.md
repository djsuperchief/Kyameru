---
layout: page
title: Chain Link Route Specific DI
parent: Design Decisions
---

# Chain Link Dependency Injection Isolation
## Overview

There are certain circumstances where chain links will need to pull something from
the DI container but it will need to be specific to that chain. The reason the DI container is being used is that the way the creation of components works is by either concrete creation OR using DI to register chain links and then pull them from the container at the time of building a route. For instance, the AWS components all register themselves with DI so we can leverage component creation (for things like the AWS SDK) and not have to worry about it.

Whilst developing the REST component and it's relevant chain links, several challenges were presented.

## Challenge 1 - Authentication

It is entirely possible that an implementation would use one endpoint in the `From` and another in the `To`. Defining an authentication strategy for the overall route would not be sufficient as you would need two. You may also have more than one route in the solution and so would potentially need several auth strategies.
The next part of the challenge is that there is no way of setting this in the current URI standard for Kyameru. Simply adding parameters to the query string may suffice but it looks messy and doesn't work well with the fluent nature of creating a route.

## Challenge 2 - Config

Config is an interesting one in that we allow Kyameru routes to be created by config. The core is responsible for the configs structure and so we cannot add component based config entries without tightly coupling the component to the core. Moreover, how can we pull the config into each component or chain links creation without it accidentally being used by another route?

## Thoughts
### Internal Id

Kyameru has the ability to give an ID to the route. If you do not assign one, a random GUID is generated and assigned to the route (there needs to be some element of checking for duplicate ids here....on the todo list). However no chain links are ever aware of their routes ID which is by design. Kyameru knows the ID of the overall route and this maintains the seperation.
However, recent (well, semi-recent) additions to the .NET DI allow you to add a key to registered services which means you can add and pull services out of the DI container by key. Where this brings me to is if each chain link was assigned a randomly generated id and then any bits like auth, config etc that are chain link specific, were registered with that key then we can pull this out at register / creation time. Each component is responsible for how it registers the chain links with DI and so we can customise this.

The small caveat here is that config related bits still need to rely on the routes ID because there is no way for the core to identify extra elements in the config and know what chain link they are for, it will know what component they are for and so can add these extra elements to a structure in the DI with the routes ID. This does mean that the whole component will also need to know it's route ID from the core which in turn means this could eliminate the need for a chain link specific id.

### Builder Extensions

So another interesting thought is that for certain components, we will need to extend the builder methods. But the problem here is how do we loop this into the core's building process?
The core builds up an object with various properties and then only on `Build()` does it start doing the DI registration and creation. It has no awareness of the DI container at the time of construction....so even if we extend the builder we are still stuck at "how can i actually action these extensions?".

#### Callbacks / Callouts

I think the only reasonable option here is to add callbacks or callouts to the build process. This will allow components to extend the building process and then inject a delegate into the building function. It makes sense and it allows us to extend the core without too much bother.