---
layout: page
title: About
permalink: /about/
---



Kyameru is a business processing engine based off Apache Camel. The general idea is that you have a "bucket" of ready made components that you can use in a processing chain for any incoming data. You have a FROM component that starts the chain of events. Processing components that typically you build to process the data and then TO components to send the data to something.

The project is open source, built in .NET and open to contributions.

```mermaid
flowchart TD;
    FROM-->PROCESSING-1;
    PROCESSING-1-->PROCESSING-2;
    PROCESSING-2-->TO-1;
    PROCESSING-2-->TO-2;
```
