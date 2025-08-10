# Tedu Microservices Identity Project

Identity Server for Tedu Microservices Course.

- Tedu Microservice Course : [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/49/C5D7O1.html)
- Microfrontends Angular Mastery Course: [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/56/C5D7O1.html)

## Application URLs - DEVELOPMENT:

- Identity API: http://localhost:5001

## Docker Command Examples

- Create a ".env" file at the same location with docker-compose.yml file:
  COMPOSE_PROJECT_NAME=tedu_microservices_idp
- run command: `docker-compose -f docker-compose.yml up -p tedu_idp -d --remove-orphans --build`
- Run script: idp_stores.sql in DatabaseScripts/Store Procedures

## Application URLs - PRODUCTION:

## Packages References

- [https://github.com/serilog/serilog/wiki/Getting-Started](https://github.com/serilog/serilog/wiki/Getting-Started)
- [https://duendesoftware.com/products/identityserver](https://duendesoftware.com/products/identityserver)

## Install Environment

## References URLS

## Recommended Courses:

- Xây dựng hệ thống với kiến trúc Micro-service: [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/49/C5D7O1.html)
- Microfrontends Angular Mastery Course: [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/56/C5D7O1.html)
- Terraform Mastery trên Cloud Azure: Từ Cơ bản đến Nâng cao: [https://tedu.com.vn/khoa-hoc](https://tedu.com.vncourse-ref/52/C5D7O1.html)
- Làm chủ Docker để chinh phục DevOps: [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/42/C5D7O1.html)
- Tedu Exam course (MongoDb, DDD, CQRS, Identity Server): [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/43/C5D7O1.html)
- Authentication và Authorization nâng cao: [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/36/C5D7O1.html)
- Xây dựng ứng dụng web với ASP.NET Core Web API + Identity Server + Angular: [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/35/C5D7O1.htm)
- Phát triển Web App với .NET 6 (ABP Framework & Angular): [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/50/C5D7O1.html)
- Triển khai CI/CD với Azure DevOps: [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/37/C5D7O1.html)
- Thiết kế RESTful API với ASP.NET Core và Dapper ORM: [https://tedu.com.vn/khoa-hoc](https://tedu.com.vn/course-ref/24/C5D7O1.html)

## Migrations commands (cd into TeduMicroservices.IDP project):

- migration PersistedGrantDbContext: `dotnet ef migrations add "Initial_PersistedGrantDb" -c PersistedGrantDbContext -s TeduMicroservices.IDP.csproj -o Persistence/Migrations/PersistedGrantDb`
- migration ConfigurationDbContext: `dotnet ef migrations add "Initial_ConfigurationDb" -c ConfigurationDbContext -s TeduMicroservices.IDP.csproj -o Persistence/Migrations/ConfigurationDb`
- update db PersistedGrantDbContext: `dotnet ef database update -c PersistedGrantDbContext`
- update db ConfigurationDbContext: `dotnet ef database update -c ConfigurationDbContext`
- Migration TeduIdentityContext: `dotnet ef migrations add "Init_Identity" -c TeduIdentityContext -s TeduMicroservices.IDP.csproj -p ../TeduMicroservices.IDP.Infrastructure/TeduMicroservices.IDP.Infrastructure.csproj -o Persistence/Migrations`
- Update db TeduIdentityContext: `dotnet ef database update -c TeduIdentityContext -s TeduMicroservices.IDP.csproj -p ../TeduMicroservices.IDP.Infrastructure/TeduMicroservices.IDP.Infrastructure.csproj`

## Useful commands:

- Docker build (root folder): docker build -t tedu_microservice_idp:latest -f src/TeduMicroservices.IDP/Dockerfile src/.
- Update migration (root folder):
  - `dotnet ef database update -c PersistedGrantDbContext -s src/TeduMicroservices.IDP/TeduMicroservices.IDP.csproj --connection "${connection_string}"`
  - `dotnet ef database update -c ConfigurationDbContext -s src/TeduMicroservices.IDP/TeduMicroservices.IDP.csproj --connection "${connection_string}"`
  - `dotnet ef database update -c TeduIdentityContext -p src/TeduMicroservices.IDP.Infrastructure/TeduMicroservices.IDP.Infrastructure.csproj -s src/TeduMicroservices.IDP/TeduMicroservices.IDP.csproj --connection "${connection_string}"`

## https certificate with Docker Compose:

- Create a .pfx file:
  - MacOS: dotnet dev-certs https -ep ${HOME}/.aspnet/https/tedu-idp.pfx -p password!
  - WindowsOS: dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\tedu-idp.pfx -p password!
- Trust the file: dotnet dev-certs https --trust

## Docker hub:
- Ref from video: (https://tedu.com.vn/video/714-build-cicd-pipline-for-idp-service-1853.html)
- docker login: docker login
- Go to docker hub, create a new repository: tedu-microservice-idp
- At root folder:
  - `docker build -t {your_docker_account}/tedu-microservice-idp:latest -f src/TeduMicroservices.IDP/Dockerfile src/.`
  - `docker push {your_docker_account}/tedu-microservice-idp:latest`
