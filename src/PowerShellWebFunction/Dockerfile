#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PowerShellWebFunction.csproj", "PowerShellWebFunction/"]
RUN dotnet restore "PowerShellWebFunction/PowerShellWebFunction.csproj"
WORKDIR "/src/PowerShellWebFunction"
COPY . .
RUN dotnet build "PowerShellWebFunction.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PowerShellWebFunction.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PowerShellWebFunction.dll"]
