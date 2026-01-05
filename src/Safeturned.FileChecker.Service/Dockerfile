FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG CHECKER_VERSION=0.0.0.0
ARG CHECKER_VERSION_SUFFIX=
WORKDIR /src

COPY ["Safeturned.FileChecker.Service/Safeturned.FileChecker.Service.csproj", "Safeturned.FileChecker.Service/"]
COPY ["Safeturned.FileChecker/Safeturned.FileChecker.csproj", "Safeturned.FileChecker/"]
RUN dotnet restore "Safeturned.FileChecker.Service/Safeturned.FileChecker.Service.csproj"

COPY . .
WORKDIR "/src/Safeturned.FileChecker.Service"
RUN dotnet build "Safeturned.FileChecker.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:CheckerVersion=$CHECKER_VERSION /p:CheckerVersionSuffix=$CHECKER_VERSION_SUFFIX

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG CHECKER_VERSION=0.0.0.0
ARG CHECKER_VERSION_SUFFIX=
RUN dotnet publish "Safeturned.FileChecker.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:CheckerVersion=$CHECKER_VERSION /p:CheckerVersionSuffix=$CHECKER_VERSION_SUFFIX

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5080
USER $APP_UID
ENTRYPOINT ["dotnet", "Safeturned.FileChecker.Service.dll"]
