#!/usr/bin/env bash

ipAddress=$(dig @resolver1.opendns.com ANY myip.opendns.com +short)

# Publish and Upload to bucket Webapi application
aws cloudformation create-stack \
    --stack-name webapi-dotnet-Yury-Kabernik-Berazouski \
    --template-body file://$PWD/aws/templates/template.dev.yaml \
    --parameters ParameterKey=MyIpParameter,ParameterValue=$ipAddress \
    --tags project=cloudx
