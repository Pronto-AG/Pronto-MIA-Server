# Pronto-MIA Server
[![Server CI](https://github.com/Pronto-AG/Informbob-Server/actions/workflows/main.yml/badge.svg)](https://github.com/Pronto-AG/Informbob-Server/actions/workflows/main.yml)

This is the repository for the server component of the Pronto-MIA application. The application is used by the Pronto AG to deliver information to it's employees and handle business processes within the company.

## Settings
All available settings for the application can be defined within three json files:
- `appsettings.json`
    - Contains settings relevant for all environments. Settings which do not depend on the environment the application is run in are defined here.
- `appsettings.Development.json`
    - Contains the settings that are used within a development context. Those settings will not be loaded if the application is run in production mode.
- `appsettings.Production.json`
    - Contains the settings required for the application to run in production mode. Those settings will not be loaded if the application is run in development mode.

### Available options


## Development setup
In order to be able to adjust the application server you will first need to setup the development environment correctly. In order to do this the following prerequisites need to be installed:
1. .net 5.0
2. Docker
3. Docker-Compose
4. Doxygen (Only if documentation generation is needed)

After the prerequisites are installed on your system you might initalize the development environment by following these steps:
1. Clone the repository
2. Change your working directory into the repository
3. Run `docker-compose -f docker-compose.dev.yml up`
    - This command will create and start a postgres database configured to work with the connection string within the `appsettings.Development.json` configuration file.
4. After the database is running it needs to be seeded in order to be usable by the application. To do this run `dotnet ef database update`. This command will create all tables and contents required by the application within the database.
5. Adjust the configuration within `appsettings.Development.json` to fit your needs. The settings `StaticFiles:ROOT_DIRECTORY` and `Firebase:CREDENTIAL_FILE` need static paths to where you want those places to be on your system. Therefore those settings need to be adjusted.
6. Now you may open the application folder `Pronto-MIA` with the .Net IDE of your choice and adjust/run the application.

## Production Setup
The easiest way to run the application in production mode is to download the prebuild docker container and map the needed configuration and authentication files into it. Three assets need to be mapped into the container as is demostrated in the `docker-compose.yml` file contained within the repository. Those assets are:
1. The configuration files `appsettings.json` and `appsettings.Production.json` needed by the application.
2. The `firebase-authentication.json` containing the authentication details needed by firebase to send messages. This file has to be mapped to the file specified in the setting `Firebase:CREDENTIAL_FILE`.
3. A directory where the application can persist files. This directory has to be mapped to the directory specified in the setting `StaticFiles:ROOT_DIRECTORY`.

Theoretically since it is a dotnet application it can be run outside of the provided docker container with any .net 5.0 compatible runtime. This approach is however not supported.