FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy
COPY Parking.WebAPI/Parking.WebAPI.csproj Parking.WebAPI/
RUN dotnet restore Parking.WebAPI/Parking.WebAPI.csproj

COPY . ./

# Build and publish a release
WORKDIR /app/Parking.WebAPI
RUN dotnet publish Parking.WebAPI.csproj -c Release -o /out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Parking.WebAPI.dll"]