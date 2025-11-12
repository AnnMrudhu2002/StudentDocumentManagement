# Step 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all files
COPY . .

# Publish the project (NOT solution)
RUN dotnet publish StudentDocumentManagement/StudentDocumentManagement.csproj -c Release -o /app/publish

# Step 2: Run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 for Render
EXPOSE 8080

# Start your app
ENTRYPOINT ["dotnet", "StudentDocumentManagement.dll"]
