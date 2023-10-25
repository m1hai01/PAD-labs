FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY UserServer.csproj UserServer.csproj
RUN dotnet restore UserServer.csproj

COPY . .
WORKDIR /src
RUN dotnet build UserServer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish UserServer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "UserServer.dll"]

