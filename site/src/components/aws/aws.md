---
layout: page
title: AWS
has_children: true
parent: Components
permalink: /components/aws
---

{: .fs-6 .fw-300 }

# Config
Kyameru is not responsible for configuring any AWS credentials or how to connect to AWS. It is expected that the configuration is provided by the host application and Dependency Injection will then expose the config to each AWS component.

Each component, as part of it's injection process will attempt to add a transient of its required AWS client. It is assumed that the host application will have already done this but allows for cases where it has not been done. Configuration validation is not done as it is expected to be handled by the host application.

# Local Testing
In the source you will find a docker stack containing a terraform project and Localstack. Although it is accepted that this is no real replacement for an actual AWS stack, it serves as a good integration test suite.