#!/usr/bin/env bash

configuration=Release
applicationSolution=DotnetWebapi

# publish Webapi application
dotnet publish "./$applicationSolution" --configuration $configuration 

# deploy to application s3 bucket
aws s3 sync "./$applicationSolution/bin/$configuration/net8.0/publish" s3://s3-dotnet-webapi/
