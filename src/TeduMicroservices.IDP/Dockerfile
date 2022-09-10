FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TeduMicroservices.IDP/TeduMicroservices.IDP.csproj", "TeduMicroservices.IDP/"]
RUN dotnet restore "TeduMicroservices.IDP.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "TeduMicroservices.IDP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeduMicroservices.IDP.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeduMicroservices.IDP.dll"]
