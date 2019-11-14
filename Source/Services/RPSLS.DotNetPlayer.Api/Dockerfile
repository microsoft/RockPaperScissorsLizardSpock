FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
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