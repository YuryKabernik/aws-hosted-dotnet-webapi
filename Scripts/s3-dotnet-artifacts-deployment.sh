#!/usr/bin/env bash
# sh ./s3-dotnet-artifacts-deployment.sh <profile-id>

profile=$1

path="dynamic-app"

dotnet publish -c Release --artifacts-path $path

aws s3 sync "./$path/publish/dotnet-intermediate-mentoring-program/release" s3://s3-dotnet-webapi/ --profile $profile
