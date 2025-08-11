# Estágio 1: Build da Aplicação
# Usamos a imagem completa do SDK para compilar o projeto em modo Release.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências.
COPY ["scheduler.csproj", "./"]
RUN dotnet restore "./scheduler.csproj"

# Copia o resto do código-fonte.
COPY . .
WORKDIR "/src"

# Publica a aplicação em modo Release, que é otimizado para produção.
RUN dotnet publish "scheduler.csproj" -c Release -o /app/publish

# ---

# Estágio 2: Imagem Final
# Usamos a imagem ASP.NET, que é muito menor e mais segura.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Copia apenas os arquivos publicados do estágio de build.
COPY --from=build /app/publish .

# Expõe apenas a porta que a aplicação realmente usa.
EXPOSE 8080

# Define o ponto de entrada para rodar a aplicação.
ENTRYPOINT ["dotnet", "scheduler.dll"]
