FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#USER root
#RUN apt update
#RUN apt-get -y install qemu-user qemu-user-static gcc-aarch64-linux-gnu binutils-aarch64-linux-gnu binutils-aarch64-linux-gnu-dbg build-essential
#USER $APP_UID
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cultiv.csproj", "."]
RUN dotnet restore "./Cultiv.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./Cultiv.csproj" -c $BUILD_CONFIGURATION -o /app/build 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cultiv.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# We need to make sure that the user running the app has write access to the umbraco folder, in order to write logs and other files.
# Since these are volumes they are created as root by the docker daemon.
USER root
RUN mkdir umbraco
RUN mkdir umbraco/Data
RUN mkdir umbraco/Data/TEMP
RUN mkdir umbraco/Logs
RUN mkdir wwwroot/media
RUN chown $APP_UID umbraco --recursive
RUN chown $APP_UID wwwroot/media --recursive
RUN chown $APP_UID Views --recursive
RUN chown $APP_UID uSync --recursive
USER $APP_UID
ENTRYPOINT ["dotnet", "Cultiv.dll"]
