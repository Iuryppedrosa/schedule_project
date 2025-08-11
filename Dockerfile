# Est�gio de build para depura��o
# Usa a imagem do SDK para compilar e publicar a aplica��o em modo Debug
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as depend�ncias primeiro, usando o cache
COPY ["scheduler.csproj", "./"]
RUN dotnet restore "scheduler.csproj"

# Copia todos os outros arquivos do projeto e publica a aplica��o
COPY . .
WORKDIR "/src"
RUN dotnet publish "scheduler.csproj" -c Debug -o /app/publish

# Est�gio de execu��o para depura��o
# Usa uma imagem menor, otimizada para rodar a aplica��o
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Instala��o do vsdbg (Visual Studio Debugger)
RUN apt-get update && \
    apt-get install -y unzip && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

# Copia os arquivos publicados do est�gio de build para o diret�rio de execu��o
COPY --from=build /app/publish .

# Expor as portas 8080 (HTTP) e 5001 (HTTPS e para o debugger)
EXPOSE 8080
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:8080

# Configura��o para que o cont�iner possa ser debugado
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true
ENTRYPOINT ["dotnet", "scheduler.dll"]
