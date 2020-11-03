ARG sdkTag=5.0
ARG runtimeTag=5.0
ARG image=mcr.microsoft.com/dotnet/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/sdk

FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 80

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src/project
COPY ./RPSLS.Game.Api ./RPSLS.Game.Api/
COPY ./RPSLS.Game.Multiplayer ./RPSLS.Game.Multiplayer/
#COPY . .
RUN dotnet build ./RPSLS.Game.Api/RPSLS.Game.Api.csproj -c Release -o /app
#RUN dotnet build "RPSLS.Game.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish ./RPSLS.Game.Api/RPSLS.Game.Api.csproj -c Release -o /app
#RUN dotnet publish "RPSLS.Game.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RPSLS.Game.Api.dll"]