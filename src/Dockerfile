FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["IdentityManagementService.csproj", "./"]
RUN dotnet restore "IdentityManagementService.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "IdentityManagementService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IdentityManagementService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TIKSN.Lionize.IdentityManagementService.dll"]
