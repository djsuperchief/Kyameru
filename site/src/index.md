---
# Feel free to add content and custom Front Matter to this file.
# To modify the layout, see https://jekyllrb.com/docs/themes/#overriding-theme-defaults

layout: home
title: Home
permalink: /
nav_order: 1
---

{: .text-center }
![kyameru logo](/assets/img/logo.png)

{: .warning }
> Welcome to the Kyameru documentation home. This is still relatively new and will constantly be being updated. For now, please visit the main GitHub repository: [Kyameru Source](https://github.com/djsuperchief/Kyameru)

## Status
[![Build](https://github.com/djsuperchief/Kyameru/actions/workflows/build.yml/badge.svg)](https://github.com/djsuperchief/Kyameru/actions/workflows/build.yml)
[![Coverage Status](https://coveralls.io/repos/github/djsuperchief/Kyameru/badge.svg?branch=main)](https://coveralls.io/github/djsuperchief/Kyameru?branch=main)
![GitHub Tag](https://img.shields.io/github/v/tag/djsuperchief/kyameru)
![Latest Github Release](https://img.shields.io/github/v/release/djsuperchief/kyameru?include_prereleases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Kyameru

Kyameru is a processing engine built with heavy inspiration from Apache Camel. The general idea is that you have a "bucket" of ready made components that you can use in a processing chain for any incoming data. You have a FROM component that starts the chain of events. Processing components that typically you build to process the data and then TO components to send the data to something.

## Get Started

Currently, the project is not being distributed through NuGet. When the project reaches a full beta stage (with a number of MVP components), it will be pushed to NuGet and ready to be used. Until then, hold tight. Releases, however, are still being published and can continue to be used until Nuget packages are released.