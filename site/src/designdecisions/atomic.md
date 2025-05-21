---
layout: page
title: Atomic
parent: Design Decisions
---

# Atomic Chain Link Removal
## Overview

The atomic chain link was designed to allow the `From` chain link to have some form of atomic execution built in so that users didn't need to worry about it. In theory it would mean something like an FTP/SFTP/S3 component wouldn't delete a file in the source until the entire route had been processed.
In reality, none of the current components actually implement the `IAtomicLink` interface so it is going entirely unused. Not only that, the components are designed to provide a mechanism to chain services / providers together and then processing done on the messages. It should not be up to the components to decide what is atomic and what is not.

## Alternatives
Post processing has been added to `To` chain links and this only happens once the `To` link has been executed. In this way, we already have a user driven event happening right at the end of the route which can provide that atomic nature. Any atomicity should be driven by the users implementation and not a decision that is to be made by the component.

## Decision
The atomic part of components will be removed and not replaced as an adequate alternative (`To` post processing) already exists and will be removed before version 1.0.