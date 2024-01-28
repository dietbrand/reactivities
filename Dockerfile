FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder

WORKDIR /app
EXPOSE 8080

COPY "Reactivities.sln" .
COPY "API/API.csproj" "API/API.csproj"
COPY "Application/Application.csproj" "Application/Application.csproj"
COPY "Persistence/Persistence.csproj" "Persistence/Persistence.csproj"
COPY "Domain/Domain.csproj" "Domain/Domain.csproj"
COPY "Infrastructure/Infrastructure.csproj" "Infrastructure/Infrastructure.csproj"

RUN dotnet restore "Reactivities.sln"

COPY . .
RUN dotnet publish  -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=builder /app/out .
ENTRYPOINT [ "dotnet","API.dll" ]