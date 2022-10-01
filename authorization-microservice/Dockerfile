FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["authorization-microservice/authorization-microservice.csproj", "authorization-microservice/"]
RUN dotnet restore "authorization-microservice/authorization-microservice.csproj"
COPY . .
WORKDIR "/src/authorization-microservice"
RUN dotnet build "authorization-microservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "authorization-microservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "authorization-microservice.dll"]
