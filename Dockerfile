# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["StretchFitnessHub.csproj", "./"]
RUN dotnet restore "./StretchFitnessHub.csproj"
COPY . .
RUN dotnet publish "StretchFitnessHub.csproj" -c Release -o /app/publish

# Stage 2: Run the app
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "StretchFitnessHub.dll"]
