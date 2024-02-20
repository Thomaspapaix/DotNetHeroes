# Utilisation de l'image .NET SDK 8.0 pour l'étape de construction
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copie du fichier projet et restauration des dépendances
COPY *.csproj ./
RUN dotnet restore

# Copie du code source et construction de l'application
COPY . ./
RUN dotnet publish -c Release -o out

# Utilisation de l'image .NET runtime pour l'étape finale
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .

# Exposition du port de l'application
EXPOSE 80

# Définition de la variable d'environnement ASPNETCORE_ENVIRONMENT pour indiquer que nous sommes en environnement Docker
ENV ASPNETCORE_ENVIRONMENT="Docker"

# Exécution des migrations au démarrage de l'application
ENTRYPOINT ["dotnet", "TodoApi.dll", "migrate"]
