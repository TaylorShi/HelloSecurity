#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Tesla.Order.HackedSite/Tesla.Order.HackedSite.csproj", "Tesla.Order.HackedSite/"]
RUN dotnet restore "Tesla.Order.HackedSite/Tesla.Order.HackedSite.csproj"
COPY . .
WORKDIR "/src/Tesla.Order.HackedSite"
RUN dotnet build "Tesla.Order.HackedSite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tesla.Order.HackedSite.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tesla.Order.HackedSite.dll"]