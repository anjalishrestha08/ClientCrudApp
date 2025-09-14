# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# restore
COPY ["ClientCrudApp/ClientCrudApp.csproj", "ClientCrudApp/"]
RUN dotnet restore "ClientCrudApp/ClientCrudApp.csproj"

# copy remaining source
COPY . .

WORKDIR /src/ClientCrudApp
RUN dotnet build "ClientCrudApp.csproj" -c Release -o /app/build

# Stage 2: Publish Stage
FROM build AS publish
RUN dotnet publish "ClientCrudApp.csproj" -c Release -o /app/publish

# Stage 3: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ClientCrudApp.dll"]
