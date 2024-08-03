#!/usr/bin/env bash
# sh ./ec2-webapi-deployment-from-s3.sh

# install dotnet 8.0 runtime
dnf install -y aspnetcore-runtime-8.0

# target hosting directory
path=/var/www/webapi

# download webapi from the S3 bucket
aws s3 cp s3://s3-dotnet-webapi/ $path --recursive

# copy the service file
mv -f $path/kestrel-webapi.service /etc/systemd/system/kestrel-webapi.service

# the service name
srvName=kestrel-webapi.service

# enable & start the service
sudo systemctl enable $srvName
sudo systemctl start $srvName
sudo systemctl status $srvName

# list logs
# sudo journalctl -fu kestrel-webapi.service