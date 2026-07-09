FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY shared/Shop.Events/Shop.Events.csproj shared/Shop.Events/
COPY services/Seller/src/Seller.Domain/Seller.Domain.csproj services/Seller/src/Seller.Domain/
COPY services/Seller/src/Seller.Application/Seller.Application.csproj services/Seller/src/Seller.Application/
COPY services/Seller/src/Seller.Infrastructure/Seller.Infrastructure.csproj services/Seller/src/Seller.Infrastructure/
COPY services/Seller/src/Seller.API/Seller.API.csproj services/Seller/src/Seller.API/

RUN dotnet restore services/Seller/src/Seller.API/Seller.API.csproj

COPY shared/Shop.Events/ shared/Shop.Events/
COPY services/Seller/src/Seller.Domain/ services/Seller/src/Seller.Domain/
COPY services/Seller/src/Seller.Application/ services/Seller/src/Seller.Application/
COPY services/Seller/src/Seller.Infrastructure/ services/Seller/src/Seller.Infrastructure/
COPY services/Seller/src/Seller.API/ services/Seller/src/Seller.API/

RUN dotnet publish services/Seller/src/Seller.API/Seller.API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Seller.API.dll"]