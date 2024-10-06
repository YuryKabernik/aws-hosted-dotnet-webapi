#!/usr/bin/env bash

destination="s3://s3-dotnet-webapi-functions/"
currentdir=$(dirname $0)

# Publish and Upload to bucket Webapi application
./${currentdir}/publish-rest-service.sh

# Pack and Publish lambda function 'DataConsistencyFunction'
./${currentdir}/publish-function.sh \
    -p DataConsistencyFunction \
    -d $destination

# Pack and Publish lambda function 'UploadsNotificationFunction'
./${currentdir}/publish-function.sh \
    -n UploadsNotificationFunction \
    -p LambdaSnsSqsNotification \
    -d $destination

# Pack and Publish lambda function 'S3LogsFunction'
./${currentdir}/publish-function.sh \
    -p S3LogsFunction \
    -d $destination
