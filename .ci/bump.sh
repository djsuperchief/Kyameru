#!/usr/bin/env sh

# this is not to be run, this is just for testing.

RELEASE_TYPE="false"

version=$(python ./.ci/getversion.py $RELEASE_TYPE 2>&1)
echo $version

