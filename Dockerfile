FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY transfer_service.csproj .
RUN dotnet restore transfer_service.csproj

COPY . .
RUN dotnet publish transfer_service.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "transfer_service.dll"]