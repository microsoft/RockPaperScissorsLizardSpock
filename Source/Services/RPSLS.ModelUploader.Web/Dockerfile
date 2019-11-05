ARG sdkTag=3.0
ARG runtimeTag=3.0
ARG image=mcr.microsoft.com/dotnet/core/aspnet
ARG sdkImage=mcr.microsoft.com/dotnet/core/sdk


FROM ${image}:${runtimeTag} AS base
WORKDIR /app
EXPOSE 8080

FROM ${sdkImage}:${sdkTag} AS build
WORKDIR /src
COPY ["RPSLS.ModelUploader.Web.csproj", "."]
RUN dotnet restore "RPSLS.ModelUploader.Web.csproj"
COPY . .
RUN dotnet build "RPSLS.ModelUploader.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPSLS.ModelUploader.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY initialModel /initialModel/
COPY entrypoint.sh .
RUN chmod +x /app/entrypoint.sh
CMD /app/entrypoint.sh