#!/usr/bin/env bash

# Create a stack for NotificationLambda
aws cloudformation create-stack \
    --stack-name webapi-dotnet-UploadsNotificationLambda \
    --template-body file://$PWD/aws/templates/UploadsNotificationLambda.yaml \
    --tags Key=project,Value=cloudx
