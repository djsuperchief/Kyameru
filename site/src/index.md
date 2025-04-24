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

{: .note }
> Welcome to the Kyameru documentation home. This is still relatively new and will constantly be being updated. The product is still in it's beta phase and is available on NuGet.

## Status
[![Build](https://github.com/djsuperchief/Kyameru/actions/workflows/build.yml/badge.svg)](https://github.com/djsuperchief/Kyameru/actions/workflows/build.yml)
[![Coverage Status](https://coveralls.io/repos/github/djsuperchief/Kyameru/badge.svg?branch=main)](https://coveralls.io/github/djsuperchief/Kyameru?branch=main)
![GitHub Tag](https://img.shields.io/github/v/tag/djsuperchief/kyameru)
![Latest Github Release](https://img.shields.io/github/v/release/djsuperchief/kyameru?include_prereleases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Kyameru

Kyameru is a processing engine built with heavy inspiration from Apache Camel. The general idea is that you have a "bucket" of ready made components that you can use in a processing chain for any incoming data. You have a FROM component that starts the chain of events. Processors that typically you build to process the data and then TO components to send the data to something.

## Get Started

All components are available from NuGet or you can download the source directly and reference it within your projects. The recommended way is to use NuGet; follow instructions [here](getting_started.md).