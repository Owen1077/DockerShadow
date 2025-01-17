#!/bin/sh
GIT_VERSION=$(git describe --tags)
docker build -t registry.aff.ng/DockerShadow_core:$GIT_VERSION -f DockerShadow/Dockerfile .
docker build -t registry.aff.ng/DockerShadow_admin:$GIT_VERSION -f DockerShadow/Dockerfile .
docker push registry.aff.ng/DockerShadow_core:$GIT_VERSION
docker push registry.aff.ng/DockerShadow_admin:$GIT_VERSION

