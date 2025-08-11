# Est�gio 1: Build da Aplica��o
# Usamos a imagem completa do SDK para compilar o projeto em modo Release.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as depend�ncias.
COPY ["scheduler.csproj", "./"]
RUN dotnet restore "./scheduler.csproj"

# Copia o resto do c�digo-fonte.
COPY . .
WORKDIR "/src"

# Publica a aplica��o em modo Release, que � otimizado para produ��o.
RUN dotnet publish "scheduler.csproj" -c Release -o /app/publish

# ---

# Est�gio 2: Imagem Final
# Usamos a imagem ASP.NET, que � muito menor e mais segura.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Copia apenas os arquivos publicados do est�gio de build.
COPY --from=build /app/publish .

# Exp�e apenas a porta que a aplica��o realmente usa.
EXPOSE 8080

# Define o ponto de entrada para rodar a aplica��o.
ENTRYPOINT ["dotnet", "scheduler.dll"]
