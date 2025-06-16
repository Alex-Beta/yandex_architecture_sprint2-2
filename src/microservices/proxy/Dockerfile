# Используем официальный образ SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем файлы проекта и восстанавливаем зависимости
COPY ["./cinemaabyss-proxy-service/*.csproj", "./"]
RUN dotnet restore

# Копируем все остальные файлы и собираем приложение
COPY ./cinemaabyss-proxy-service/ .
RUN dotnet build -c Debug -o /app/build
RUN dotnet publish -c Debug -o /app/publish

# Создаем финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "cinemaabyss-proxy-service.dll"]
