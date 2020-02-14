ARG sdkTag=3.1
ARG runtimeTag=3.1
ARG image=mcr.microsoft.com/dotnet/core/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/core/sdk

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