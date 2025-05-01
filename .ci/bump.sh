#!/usr/bin/env sh

# this is not to be run, this is just for testing.

RELEASE_TYPE="false"
VERSION=0.0.1

version=$(echo $VERSION | python ./.ci/bump.py 2>&1)
echo $version

