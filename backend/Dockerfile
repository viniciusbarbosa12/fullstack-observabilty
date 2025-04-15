FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./EmployeeManagement.Api/*.csproj ./EmployeeManagement.Api/
RUN dotnet restore ./EmployeeManagement.Api/EmployeeManagement.Api.csproj

COPY . .
WORKDIR /src/EmployeeManagement.Api
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]
