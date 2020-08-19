#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CastonFactory/CastonFactory.csproj", "CastonFactory/"]
COPY ["OneSignal.API/OneSignal.API.csproj", "OneSignal.API/"]
COPY ["CastonFactory.Data/CastonFactory.Data.csproj", "CastonFactory.Data/"]
RUN dotnet restore "CastonFactory/CastonFactory.csproj"
COPY . .
WORKDIR "/src/CastonFactory"
RUN dotnet build "CastonFactory.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CastonFactory.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CastonFactory.dll"]