FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
RUN dotnet restore "./Api/Api.csproj"
COPY . . 
WORKDIR "/src/Api"
RUN dotnet build "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app


COPY ./Api/wait-for-it.sh /wait-for-it.sh

USER root

RUN chmod +x /wait-for-it.sh

USER app

COPY --from=publish /app/publish .

ENTRYPOINT ["/wait-for-it.sh", "sqlserver-db:1433", "--", "dotnet", "Api.dll"]
