#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Set the environment variable
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DockerShadow/DockerShadow.csproj", "DockerShadow/"]
COPY ["DockerShadow.Infrastructure/DockerShadow.Infrastructure.csproj", "DockerShadow.Infrastructure/"]
COPY ["DockerShadow.Core/DockerShadow.Core.csproj", "DockerShadow.Core/"]
COPY ["DockerShadow.Persistence/DockerShadow.Persistence.csproj", "DockerShadow.Persistence/"]
COPY ["DockerShadow.Domain/DockerShadow.Domain.csproj", "DockerShadow.Domain/"]
RUN dotnet restore "./DockerShadow/DockerShadow.csproj"
COPY . .
WORKDIR "/src/DockerShadow"
RUN dotnet build "./DockerShadow.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DockerShadow.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerShadow.dll"]