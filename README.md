# Kyameru
![logo](./docs/logo.png)

[![Build](https://github.com/djsuperchief/Kyameru/actions/workflows/build.yml/badge.svg)](https://github.com/djsuperchief/Kyameru/actions/workflows/build.yml)
[![Coverage Status](https://coveralls.io/repos/github/djsuperchief/Kyameru/badge.svg?branch=main)](https://coveralls.io/github/djsuperchief/Kyameru?branch=main)
![GitHub Tag](https://img.shields.io/github/v/tag/djsuperchief/kyameru)
![Latest Github Release](https://img.shields.io/github/v/release/djsuperchief/kyameru?include_prereleases)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Kyameru.Core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Kyameru is a lightweight, extensible integration framework for .NET, built around simple, composable routes. It enables you to connect systems, trigger events, transform data, and orchestrate workflows using a clean, fluent API.

Whether you're wiring together microservices, polling external systems, or building message‑driven pipelines, Kyameru provides a predictable and flexible foundation.

## 📚 Documentation

Full documentation, examples, and component reference are available at:

👉 **[https://djsuperchief.github.io/Kyameru/](https://djsuperchief.github.io/Kyameru/)**

The documentation site includes:
- Getting started guides
- Component reference (File, AWS, REST, etc.)
- Route building concepts
- Authentication and configuration
- Advanced usage and extension points

## 🚀 Features

- **Route‑based integration model**
- **REST “from” and “to” components** with authentication
- **File, AWS, and other built‑in components**
- **Extensible architecture** for custom components
- **Dependency injection support**
- **Lightweight and easy to embed**
- **Modern .NET support**

## 🧩 Quick Start

A minimal example showing how to build a simple route:

```csharp
var route = Route.Create()
    .From("timer://5s")
    .To("log://info");
```

Or a REST‑based route:

```csharp
var route = Route.Create()
    .From("rest://get:https://api.example.com/data")
    .To("log://info");
```

See the documentation for full examples and configuration options.

## 📦 Installation

Kyameru is available on NuGet:

```bash
dotnet add package Kyameru
```

Additional components (REST, File, etc.) are available as separate packages.

## 🛠️ Components

Kyameru ships with a growing set of components, including:

- **REST** (inbound & outbound)
- **File**
- **AWS**
- **Slack**
- **FTP**

Each component is documented in detail at:  
[https://djsuperchief.github.io/Kyameru/components](https://djsuperchief.github.io/Kyameru/components)

## 🧱 Architecture Overview

Kyameru is built around four core ideas:

- **From** — where messages originate
- **Process** - where custom logic is run
- **To** — where messages are sent
- **Route** — the pipeline connecting them


## 🧪 Contributing

Kyameru welcomes contributions!

The repository includes:
- A development container
- A full test suite
- Documentation tooling

The contributing guide will be available soon.

## 📄 Release Notes

Kyameru 1.0 introduces:
- A fully re‑implemented REST component
- Improved routing engine
- Better dependency injection
- New monitoring and shutdown behaviour
- Expanded test coverage
- Updated documentation site

Full release notes are available at:  
[https://djsuperchief.github.io/Kyameru/releases/1.0.html](https://djsuperchief.github.io/Kyameru/releases/1.0.html)

## 📜 License

Kyameru is released under the MIT License.

## ⭐ Support the Project

If Kyameru helps you build something great, consider starring the repository to support its development.
