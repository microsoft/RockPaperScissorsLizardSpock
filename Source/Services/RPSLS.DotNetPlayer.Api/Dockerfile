ARG sdkTag=3.1
ARG runtimeTag=3.1
ARG image=mcr.microsoft.com/dotnet/core/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/core/sdk

FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src
COPY ["RPSLS.DotNetPlayer.Api.csproj", ""]
RUN dotnet restore "./RPSLS.DotNetPlayer.Api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "RPSLS.DotNetPlayer.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RPSLS.DotNetPlayer.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RPSLS.DotNetPlayer.Api.dll"]