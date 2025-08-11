# Estágio de build para depuração
# Usa a imagem do SDK para compilar e publicar a aplicação em modo Debug
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências primeiro, usando o cache
COPY ["scheduler.csproj", "./"]
RUN dotnet restore "scheduler.csproj"

# Copia todos os outros arquivos do projeto e publica a aplicação
COPY . .
WORKDIR "/src"
RUN dotnet publish "scheduler.csproj" -c Debug -o /app/publish

# Estágio de execução para depuração
# Usa uma imagem menor, otimizada para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Instalação do vsdbg (Visual Studio Debugger)
RUN apt-get update && \
    apt-get install -y unzip && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

# Copia os arquivos publicados do estágio de build para o diretório de execução
COPY --from=build /app/publish .

# Expor as portas 8080 (HTTP) e 5001 (HTTPS e para o debugger)
EXPOSE 8080
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:8080

# Configuração para que o contêiner possa ser debugado
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true
ENTRYPOINT ["dotnet", "scheduler.dll"]
