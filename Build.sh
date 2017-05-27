#!/usr/bin/env bash
set -e
dotnet restore
dotnet build ./RabbitMqPublishSubscribe/RabbitMqPublishSubscribe.csproj -c Release