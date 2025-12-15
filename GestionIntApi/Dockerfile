# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY GestionIntApi/*.csproj ./GestionIntApi/
RUN dotnet restore

COPY . .
WORKDIR /app/GestionIntApi
RUN dotnet publish -c Release -o /out

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GestionIntApi.dll"]
