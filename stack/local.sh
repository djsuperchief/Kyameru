!#/usr/bin/env sh

podman compose up -d
podman compose wait terraform
