#!/usr/bin/env bash
# sh ./s3-dotnet-artifacts-deployment.sh <profile-id>

profile=$1
configuration=Debug

dotnet publish -c $configuration

aws s3 sync "./bin/$configuration/net8.0/publish" s3://s3-dotnet-webapi/ --profile "$profile"
