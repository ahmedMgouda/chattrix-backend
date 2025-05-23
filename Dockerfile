FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Chattrix.sln ./
COPY Chattrix.Api/Chattrix.Api.csproj Chattrix.Api/
COPY Chattrix.Application/Chattrix.Application.csproj Chattrix.Application/
COPY Chattrix.Infrastructure/Chattrix.Infrastructure.csproj Chattrix.Infrastructure/
COPY Chattrix.Core/Chattrix.Core.csproj Chattrix.Core/

RUN dotnet restore

COPY . .
RUN dotnet publish Chattrix.Api/Chattrix.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "Chattrix.Api.dll"]
