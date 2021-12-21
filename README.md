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

The following options are available and have to be set in order for the application to function.

#### Logging

- `Logging:LogLevel:Default` -> The log level which is taken by the log provider in order to filter log events.
- `Logging:LogLevel:Microsoft` -> The log level which is taken by the log provider in order to filter log events regarding the `Microsoft` namespace.
- `Logging:LogLevel:Microsoft.Hosting.Lifetime` -> The log level which is taken by the log provider in order to filter log events regarding the `Microsoft.Hosting.Lifetime` namespace.

#### ASP.NET

- `AllowedHosts`-> The hostnames which the application may bind to.

#### API

- `API:GRAPHQL_ENDPOINT` -> The endpoint which will serve the GraphQL-API. This option contains a relative url from the host's root.

#### ConnectionStrings

- `ConnectionStrings:ProntoMIADbContext` -> Option containing the postgres connection string used by the application to connect to the database.

#### StaticFiles

- `StaticFiles:ENDPOINT` -> The endpoint over which static files delivered by the application will be served. This option contains a relative url from the host's root.
- `StaticFiles:ROOT_DIRECTORY` -> The directory where static files served by the application will be stored. This option contains an absolute path to the target directory.

#### User

- `User:MIN_PASSWORD_LENGTH` -> The minimum length a users Password must have. Since the password always needs to contain a digit, a lowercase character, a uppercase character and a non alphanumeric character this value has to be over `4`. Setting this value lower than `10` is not recommendet.

#### JWT

- `JWT:ISSUER` -> The issuer used to sign authentication tokens created by this application.
- `JWT:AUDIENCE` -> Audience specified within the authentication tokens created by this application.
- `JWT:SIGNING_KEY` -> Key used to sign the authentication tokens of this application. This information is highly confidential and should be kept secret.
- `JWT:VALID_FOR_DAYS` -> Integer telling the application how long a token should remain valid.
- `JWT:REQUIRE_HTTPS` -> Defines if the authentication requires https.

#### Firebase

- `Firebase:CREDENTIAL_FILE` -> Path to the file where the credentials needed by firebase are stored. This option contains an absolute path to the file on the file system.

#### Smtp

- `Smtp:smtp_server` -> The smtp server name to establish a connection.
- `Smtp:smtp_username` -> The smtp username to authenticate at the smtp server.
- `Smtp:smtp_password` -> The smtp password to authenticate at the smtp server.
- `Smtp:smtp_port` -> The smtp port to establish a connection with the smtp server.
- `Smtp:smtp_recipient` -> Defines the recipient to which the mail will be sent.
- `Smtp:smtp_sender` -> Defines the sender from which the mail will be sent.

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
4. As soon as the database is running, it needs to be seeded in order to be usable by the application. To do this run `dotnet ef database update`. This command will create all tables and contents required by the application within the database.
5. Adjust the configuration within `appsettings.Development.json` to fit your needs. The settings `StaticFiles:ROOT_DIRECTORY` and `Firebase:CREDENTIAL_FILE` need static paths to where you want those places to be on your system. Therefore those settings need to be adjusted.
6. Now you may open the application folder `Pronto-MIA` with the .Net IDE of your choice and adjust/run the application.

## Production Setup

The easiest way to run the application in production mode is to download the prebuilt docker container and map the needed configuration and authentication files into it. Three assets need to be mapped into the container as demostrated in the `docker-compose.yml` file contained within the repository. Those assets are:

1. The configuration files `appsettings.json` and `appsettings.Production.json` needed by the application.
2. The `firebase-authentication.json` containing the authentication details needed by firebase to send messages. This file has to be mapped to the file specified in the setting `Firebase:CREDENTIAL_FILE`.
3. A directory where the application can persist files. This directory has to be mapped to the directory specified in the setting `StaticFiles:ROOT_DIRECTORY`.

Theoretically since it is a dotnet application it can be run outside of the provided docker container with any .net 5.0 compatible runtime. This approach is however not supported.

## Login

The initially created user is called `Admin` and has the password `ProntoMIA.`.
