#!/usr/bin/env bash
#
# Usage example: 
#   ./publish-function.sh
#     -p <project-folder>
#     -d <destination-s3-bucket-uri>
#     [-c <dotnet-build-configuration>]
#     [-n <lambda-name>]
#

# Default project build configuration is Release
configuration=Release

while getopts "c:p:d:n:" flag
do
    case "${flag}" in
          c) configuration=${OPTARG};;
          d) destination=${OPTARG};;
          p) project=${OPTARG};;
          n) name=${OPTARG};;
          \?) exit 1;;
    esac
done

[ -z "${project}" ] && echo "Project name is required" && exit 1;
[ -z "${destination}" ] && echo "Upload destination is required" && exit 1;

[ -z "${name}" ] && name=${project};

# create lambda deployment package for S3LogsFunction
dotnet lambda package $name \
  --package-type zip \
  --configuration $configuration \
  --project-location "./$project" \
  --output-package "./$project/bin/$name.zip"

# deploy to lambda s3 bucket
aws s3 cp "./$project/bin/$name.zip" $destination
