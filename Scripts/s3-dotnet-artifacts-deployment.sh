#!/usr/bin/env bash
# sh ./s3-dotnet-artifacts-deployment.sh <profile-id>

profile=$1

dotnet publish -c Release

aws s3 sync "./bin/Release/net8.0/publish" s3://s3-dotnet-webapi/ --profile $profile
