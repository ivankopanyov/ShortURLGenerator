#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ApiGateways/TelegramBot/TelegramBot/TelegramBot.csproj", "ApiGateways/TelegramBot/TelegramBot/"]
RUN dotnet restore "ApiGateways/TelegramBot/TelegramBot/TelegramBot.csproj"
COPY . .
WORKDIR "/src/ApiGateways/TelegramBot/TelegramBot"
RUN dotnet build "TelegramBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramBot.dll"]
