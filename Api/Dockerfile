FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY . .
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Api.dll", "--apply-migrations"]