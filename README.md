# CSharp.Data.Service
Data Service

[![CircleCI](https://circleci.com/gh/vinils/CSharp.Data.Service.svg?style=svg)](https://circleci.com/gh/vinils/CSharp.Data.Service)
<a href="https://hub.docker.com/r/vinils/csharp-data-service/builds" target="_blank">Docker Builds</a>

git clone https://github.com/vinils/CSharp.Saude.FitbitTask.git  
docker build -t vinils/csharp-data-service .  
docker run -dt -v "/home/docker/UserSecrets:/root/.microsoft/usersecrets:ro" -v "/home/docker/Https:/root/.aspnet/https:ro" -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=https://+:443;http://+:80" -P --name data-api -p 8002:80 -p 8443:443 vinils/csharp-data-service
