# ─────────────────────────────────────────────
# Stage 1: Build
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first (layer caching for NuGet restore)
COPY MeetInSport.sln ./
COPY src/MeetInSport.Domain/MeetInSport.Domain.csproj                                   src/MeetInSport.Domain/
COPY src/MeetInSport.Application/MeetInSport.Application.csproj                         src/MeetInSport.Application/
COPY src/MeetInSport.Infrastructure.Persistence/MeetInSport.Infrastructure.Persistence.csproj  src/MeetInSport.Infrastructure.Persistence/
COPY src/MeetInSport.WebApi/MeetInSport.WebApi.csproj                                   src/MeetInSport.WebApi/

# Restore NuGet packages
RUN dotnet restore

# Copy remaining source code
COPY . .

# Publish the WebApi project in Release mode
RUN dotnet publish src/MeetInSport.WebApi/MeetInSport.WebApi.csproj \
    -c Release \
    -o /publish

# ─────────────────────────────────────────────
# Stage 2: Runtime
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /publish .

# Expose the default ASP.NET Core port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "MeetInSport.WebApi.dll"]
