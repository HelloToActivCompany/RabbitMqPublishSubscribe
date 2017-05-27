#!/usr/bin/env bash
set -e
artifactsFolder="./artifacts"
if [ -d $artifactsFolder ]; then  
  rm -R $artifactsFolder
fi
revision=${TRAVIS_JOB_ID:=1}  
revision=$(printf "%04d" $revision)
ApiKey=$1
Source=$2 
packageVersion='0.0.1.'
dotnet pack ./RabbitMqPublishSubscribe/RabbitMqPublishSubscribe.csproj -c Release -o ./artifacts /p:Version=$packageVersion$revision
dotnet nuget push ./RabbitMqPublishSubscribe/artifacts/*.nupkg -k $ApiKey -s $Source 