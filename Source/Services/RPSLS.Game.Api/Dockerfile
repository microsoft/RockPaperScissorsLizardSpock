ARG sdkTag=3.0
ARG runtimeTag=3.0
ARG image=mcr.microsoft.com/dotnet/core/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/core/sdk

FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 80

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src/project
COPY . .
RUN dotnet build "RPSLS.Game.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RPSLS.Game.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RPSLS.Game.Api.dll"]