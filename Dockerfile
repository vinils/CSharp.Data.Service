#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
COPY ["UserSecrets", "/root/.microsoft/usersecrets:ro"]
COPY ["Https", "/root/.aspnet/https:ro"]
WORKDIR /src
COPY ["CSharp.Data.Service.csproj", ""]
RUN dotnet restore "./CSharp.Data.Service.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CSharp.Data.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CSharp.Data.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharp.Data.Service.dll"]