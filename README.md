# Tedu Microservices Identity Project
Identity Server for Tedu Microservices Course.
- Tedu Microservice Course : [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/khoa-hoc/xay-dung-he-thong-voi-kien-truc-micro-service-49.html)

## Application URLs - DEVELOPMENT:

- Identity API: http://localhost:5001

## Docker Command Examples
- Create a ".env" file at the same location with docker-compose.yml file:
  COMPOSE_PROJECT_NAME=tedu_identity
- run command: docker-compose -f docker-compose.yml up -d --remove-orphans --build

## Application URLs - PRODUCTION:

## Packages References

- https://github.com/serilog/serilog/wiki/Getting-Started
- https://github.com/IdentityServer/IdentityServer4.Quickstart.UI

## Install Environment

## References URLS


## Migrations commands (cd into TeduMicroservices.IDP project):
- dotnet ef database update -c PersistedGrantDbContext
- dotnet ef database update -c ConfigurationDbContext
- dotnet ef migrations add "Init_Identity" -c TeduIdentityContext -s TeduMicroservices.IDP.csproj -p ../TeduMicroservices.IDP.Infrastructure/TeduMicroservices.IDP.Infrastructure.csproj -o Persistence/Migrations
- dotnet ef database update -c TeduIdentityContext -s TeduMicroservices.IDP.csproj -p ../TeduMicroservices.IDP.Infrastructure/TeduMicroservices.IDP.Infrastructure.csproj

## Useful commands:

