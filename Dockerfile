FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .

RUN PROJECT_FILE=$(find . -name "*.csproj" | head -n 1) && \
    echo "Using project file: $PROJECT_FILE" && \
    dotnet restore "$PROJECT_FILE" && \
    dotnet publish "$PROJECT_FILE" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Takas.Api.dll"]
