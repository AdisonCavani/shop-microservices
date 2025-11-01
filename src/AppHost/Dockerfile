FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 18888
EXPOSE 18889

FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AppHost/AppHost.csproj", "AppHost/"]
COPY ["Gateway/Gateway.csproj", "Gateway/"]
COPY ["CoreShared/CoreShared.csproj", "CoreShared/"]
COPY ["ProtobufSpec/ProtobufSpec.csproj", "ProtobufSpec/"]
COPY ["ServiceDefaults/ServiceDefaults.csproj", "ServiceDefaults/"]
COPY ["NotificationService/NotificationService.csproj", "NotificationService/"]
COPY ["OrderService/OrderService.csproj", "OrderService/"]
COPY ["ProductService/ProductService.csproj", "ProductService/"]
RUN dotnet restore "AppHost/AppHost.csproj"
COPY . .
WORKDIR "/src/AppHost"
RUN dotnet build "./AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppHost.dll"]
