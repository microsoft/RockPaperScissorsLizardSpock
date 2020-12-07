ARG sdkTag=5.0
ARG runtimeTag=5.0
ARG image=mcr.microsoft.com/dotnet/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/sdk

FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 80

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src/project
COPY ["Services/RPSLS.Game/Server/RPSLS.Game.Server.csproj", "Services/RPSLS.Game/Server/"]
COPY ["Services/RPSLS.Game/Client/RPSLS.Game.Client.csproj", "Services/RPSLS.Game/Client/"]
COPY ["Services/RPSLS.Game/Shared/RPSLS.Game.Shared.csproj", "Services/RPSLS.Game/Shared/"]
RUN dotnet restore "Services/RPSLS.Game/Server/RPSLS.Game.Server.csproj"
COPY . .
WORKDIR "/src/Services/RPSLS.Game/Server"
RUN dotnet build "RPSLS.Game.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPSLS.Game.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPSLS.Game.Server.dll"]