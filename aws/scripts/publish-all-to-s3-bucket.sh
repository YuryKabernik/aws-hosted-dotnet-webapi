#!/usr/bin/env bash

configuration=Release

applicationSolution=DotnetWebapi
lambdaSolution=LambdaSnsSqsNotification

# publish main application
dotnet publish "./$applicationSolution" --configuration $configuration 

# deploy to application s3 bucket
aws s3 sync "./$applicationSolution/bin/$configuration/net8.0/publish" s3://s3-dotnet-webapi/

# create lambda deployment package for UploadsNotificationFunction
dotnet lambda package UploadsNotificationFunction \
  --package-type zip \
  --configuration $configuration \
  --project-location "./$lambdaSolution" \
  --output-package "./$lambdaSolution/bin/UploadsNotificationFunction.zip"

# deploy to lambda s3 bucket
aws s3 cp "./$lambdaSolution/bin/UploadsNotificationFunction.zip" s3://s3-dotnet-webapi-functions/

lambdaSolution=DataConsistencyFunction

# create lambda deployment package for UploadsNotificationFunction
dotnet lambda package $lambdaSolution \
  --package-type zip \
  --configuration $configuration \
  --project-location "./$lambdaSolution" \
  --output-package "./$lambdaSolution/bin/$lambdaSolution.zip"

# deploy to lambda s3 bucket
aws s3 cp "./$lambdaSolution/bin/$lambdaSolution.zip" s3://s3-dotnet-webapi-functions/

lambdaSolution=S3LogsFunction

# create lambda deployment package for S3LogsFunction
dotnet lambda package $lambdaSolution \
  --package-type zip \
  --configuration $configuration \
  --project-location "./$lambdaSolution" \
  --output-package "./$lambdaSolution/bin/$lambdaSolution.zip"

# deploy to lambda s3 bucket
aws s3 cp "./$lambdaSolution/bin/$lambdaSolution.zip" s3://s3-dotnet-webapi-functions/
