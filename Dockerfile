
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app

COPY VitaLink.Platform.API/*.csproj VitaLink.Platform.API/
RUN dotnet restore ./VitaLink.Platform.API

COPY . .
RUN dotnet publish ./VitaLink.Platform.API -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=builder /app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "VitaLink.Platform.API.dll"]
