# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY TMS.Core/TMS.Core.csproj TMS.Core/
COPY TMS.Web/TMS.Web.csproj TMS.Web/
RUN dotnet restore TMS.Web/TMS.Web.csproj

# Copy source code and build
COPY TMS.Core/ TMS.Core/
COPY TMS.Web/ TMS.Web/
WORKDIR /src/TMS.Web
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TMS.Web.dll"]
