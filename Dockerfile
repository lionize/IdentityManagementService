FROM mcr.microsoft.com/dotnet/core/sdk:3.1.405 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./ ./
RUN dotnet restore IdentityManagementService.sln

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out IdentityManagementService.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.12
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TIKSN.Lionize.IdentityManagementService.dll"]