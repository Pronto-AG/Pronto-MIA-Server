# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY Pronto-MIA/*.sln .
COPY Pronto-MIA/Pronto-MIA/*.csproj ./Pronto-MIA/
COPY Pronto-MIA/Tests/*.csproj ./Tests/
RUN dotnet sln remove ./Tests/*.csproj

# copy everything else and build app
COPY Pronto-MIA/Pronto-MIA/. ./Pronto-MIA/
WORKDIR /source/Pronto-MIA
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Pronto-MIA.dll"]
