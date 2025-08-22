# Dockerfile for .NET API
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Acord.Api/Acord.Api.csproj", "Acord.Api/"]
RUN dotnet restore "Acord.Api/Acord.Api.csproj"
COPY . .
WORKDIR "/src/Acord.Api"
RUN dotnet publish "Acord.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Acord.Api.dll"]
