#!/usr/bin/env bash
# sh ./sg-authorize-ip-address.sh <your-profile-name> <security-group-id> <region-id>

profile=$1
groupId=$2
region=$3

ipAddress=$(dig @resolver1.opendns.com ANY myip.opendns.com +short)

aws ec2 authorize-security-group-ingress --group-id $groupId --protocol tcp --port 22 --cidr $ipAddress/32 --profile $profile --region $region
aws ec2 authorize-security-group-ingress --group-id $groupId --protocol tcp --port 80 --cidr $ipAddress/32 --profile $profile --region $region
aws ec2 authorize-security-group-ingress --group-id $groupId --protocol tcp --port 443 --cidr $ipAddress/32 --profile $profile --region $region
