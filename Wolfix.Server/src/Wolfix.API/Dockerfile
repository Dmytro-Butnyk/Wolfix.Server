FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Wolfix.API/Wolfix.API.csproj", "Wolfix.API/"]
COPY ["Admin.Endpoints/Admin.Endpoints.csproj", "Admin.Endpoints/"]
COPY ["Admin.Application/Admin.Application.csproj", "Admin.Application/"]
COPY ["Admin.Domain/Admin.Domain.csproj", "Admin.Domain/"]
COPY ["Shared.Domain/Shared.Domain.csproj", "Shared.Domain/"]
COPY ["Wolfix.ServiceDefaults/Wolfix.ServiceDefaults.csproj", "Wolfix.ServiceDefaults/"]
COPY ["Admin.IntegrationEvents/Admin.IntegrationEvents.csproj", "Admin.IntegrationEvents/"]
COPY ["Shared.IntegrationEvents/Shared.IntegrationEvents.csproj", "Shared.IntegrationEvents/"]
COPY ["Identity.IntegrationEvents/Identity.IntegrationEvents.csproj", "Identity.IntegrationEvents/"]
COPY ["Shared.Application/Shared.Application.csproj", "Shared.Application/"]
COPY ["Admin.Infrastructure/Admin.Infrastructure.csproj", "Admin.Infrastructure/"]
COPY ["Shared.Infrastructure/Shared.Infrastructure.csproj", "Shared.Infrastructure/"]
COPY ["Catalog.Endpoints/Catalog.Endpoints.csproj", "Catalog.Endpoints/"]
COPY ["Catalog.Application/Catalog.Application.csproj", "Catalog.Application/"]
COPY ["Catalog.Domain/Catalog.Domain.csproj", "Catalog.Domain/"]
COPY ["Catalog.IntegrationEvents/Catalog.IntegrationEvents.csproj", "Catalog.IntegrationEvents/"]
COPY ["Customer.IntegrationEvents/Customer.IntegrationEvents.csproj", "Customer.IntegrationEvents/"]
COPY ["Media.IntegrationEvents/Media.IntegrationEvents.csproj", "Media.IntegrationEvents/"]
COPY ["Order.IntegrationEvents/Order.IntegrationEvents.csproj", "Order.IntegrationEvents/"]
COPY ["Seller.IntegrationEvents/Seller.IntegrationEvents.csproj", "Seller.IntegrationEvents/"]
COPY ["Catalog.Infrastructure/Catalog.Infrastructure.csproj", "Catalog.Infrastructure/"]
COPY ["Media.Api/Media.Api.csproj", "Media.Api/"]
COPY ["Media.Application/Media.Application.csproj", "Media.Application/"]
COPY ["Media.Domain/Media.Domain.csproj", "Media.Domain/"]
COPY ["Media.Infrastructure/Media.Infrastructure.csproj", "Media.Infrastructure/"]
COPY ["Customer.Endpoints/Customer.Endpoints.csproj", "Customer.Endpoints/"]
COPY ["Customer.Application/Customer.Application.csproj", "Customer.Application/"]
COPY ["Customer.Domain/Customer.Domain.csproj", "Customer.Domain/"]
COPY ["Customer.Infrastructure/Customer.Infrastructure.csproj", "Customer.Infrastructure/"]
COPY ["Identity.Endpoints/Identity.Endpoints.csproj", "Identity.Endpoints/"]
COPY ["Identity.Application/Identity.Application.csproj", "Identity.Application/"]
COPY ["Identity.Infrastructure/Identity.Infrastructure.csproj", "Identity.Infrastructure/"]
COPY ["Order.Endpoints/Order.Endpoints.csproj", "Order.Endpoints/"]
COPY ["Order.Application/Order.Application.csproj", "Order.Application/"]
COPY ["Order.Domain/Order.Domain.csproj", "Order.Domain/"]
COPY ["Order.Infrastructure/Order.Infrastructure.csproj", "Order.Infrastructure/"]
COPY ["Seller.Endpoints/Seller.Endpoints.csproj", "Seller.Endpoints/"]
COPY ["Seller.Application/Seller.Application.csproj", "Seller.Application/"]
COPY ["Seller.Domain/Seller.Domain.csproj", "Seller.Domain/"]
COPY ["Seller.Infrastructure/Seller.Infrastructure.csproj", "Seller.Infrastructure/"]
RUN dotnet restore "Wolfix.API/Wolfix.API.csproj"
COPY . .
WORKDIR "/src/Wolfix.API"
RUN dotnet build "./Wolfix.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./Wolfix.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wolfix.API.dll"]
