FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY services/Seller/src/Seller.Domain/Seller.Domain.csproj Seller.Domain/
COPY services/Seller/src/Seller.Application/Seller.Application.csproj Seller.Application/
COPY services/Seller/src/Seller.Infrastructure/Seller.Infrastructure.csproj Seller.Infrastructure/
COPY services/Seller/src/Seller.API/Seller.API.csproj Seller.API/
COPY shared/Shop.Events/Shop.Events.csproj Shop.Events/
RUN dotnet restore Seller.API/Seller.API.csproj

COPY services/Seller/src/ ./services/Seller/src/
COPY shared/Shop.Events/ ./shared/Shop.Events/
WORKDIR /src/services/Seller/src
RUN dotnet publish Seller.API/Seller.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Seller.API.dll"]