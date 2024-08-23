# Etapa 1: Imagem base para o runtime .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa 2: Imagem do SDK do .NET 8 para construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os arquivos .csproj e restaurar dependências
COPY ["MottuMotoRental.API/MottuMotoRental.API.csproj", "MottuMotoRental.API/"]
COPY ["MottuMotoRental.Application/MottuMotoRental.Application.csproj", "MottuMotoRental.Application/"]
COPY ["MottuMotoRental.Core/MottuMotoRental.Core.csproj", "MottuMotoRental.Core/"]
COPY ["MottuMotoRental.Infrastructure/MottuMotoRental.Infrastructure.csproj", "MottuMotoRental.Infrastructure/"]
RUN dotnet restore "MottuMotoRental.API/MottuMotoRental.API.csproj"

# Copiar o restante do código e compilar a aplicação
COPY . .
WORKDIR "/src/MottuMotoRental.API"
RUN dotnet build "MottuMotoRental.API.csproj" -c Release -o /app/build

# Etapa 3: Publicar a aplicação
FROM build AS publish
RUN dotnet publish "MottuMotoRental.API.csproj" -c Release -o /app/publish

# Etapa 4: Imagem final com runtime e configuração de execução
FROM base AS final
WORKDIR /app

# Remova qualquer conteúdo pré-existente no diretório de destino
RUN rm -rf /app/*

# Copiar os arquivos publicados para a imagem final
COPY --from=publish /app/publish .

# Copiar o script wait-for-it.sh para dentro do contêiner
COPY wait-for-it.sh /app/wait-for-it.sh
RUN chmod +x /app/wait-for-it.sh

# Definir o comando de inicialização com wait-for-it para aguardar os serviços
CMD ["/app/wait-for-it.sh", "db:5432", "--timeout=60", "--", \
     "/app/wait-for-it.sh", "rabbitmq:5672", "--timeout=60", "--", \
     "/app/wait-for-it.sh", "mongodb:27017", "--timeout=60", "--", \
     "dotnet", "MottuMotoRental.API.dll"]



