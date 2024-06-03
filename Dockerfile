FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore SortBlazorApp.sln
# Build and publish a release
RUN dotnet publish SortBlazorApp.sln -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
EXPOSE 8080
EXPOSE 8081
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "SortBlazorApp.dll"]