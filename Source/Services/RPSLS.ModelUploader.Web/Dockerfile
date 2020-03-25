ARG sdkTag=3.1
ARG runtimeTag=3.1
ARG image=mcr.microsoft.com/dotnet/core/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/core/sdk


FROM ${image}:${runtimeTag} AS base
WORKDIR /app

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src
COPY ./RPSLS.ModelUploader.Web/RPSLS.ModelUploader.Web.csproj .
RUN dotnet restore "RPSLS.ModelUploader.Web.csproj"
COPY ./RPSLS.ModelUploader.Web/ .
RUN dotnet build "RPSLS.ModelUploader.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPSLS.ModelUploader.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /src/initialModel/ /initialModel/
COPY --from=publish /src/entrypoint.sh .
RUN chmod +x /app/entrypoint.sh
CMD /app/entrypoint.sh