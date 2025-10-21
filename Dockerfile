# 🧩 Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# .csproj dosyasını kopyala ve bağımlılıkları indir
COPY *.csproj ./
RUN dotnet restore

# Geri kalan dosyaları kopyala ve build et
COPY . ./
RUN dotnet publish -c Release -o out

# 🧩 Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Render için port belirt
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "ai_backend_dotnet.dll"]
