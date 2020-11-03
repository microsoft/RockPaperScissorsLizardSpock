ARG sdkTag=5.0
ARG runtimeTag=5.0
ARG image=mcr.microsoft.com/dotnet/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/sdk

FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 80

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src/project
COPY ./RPSLS.Web .
RUN dotnet build "RPSLS.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RPSLS.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RPSLS.Web.dll"]