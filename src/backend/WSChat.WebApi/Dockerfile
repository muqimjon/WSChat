FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

COPY ./src/backend/WSChat.WebApi/WSChat.WebSocketApi.csproj ./src/backend/WSChat.WebSocket/
RUN dotnet restore ./src/backend/WSChat.WebSocket/WSChat.WebSocketApi.csproj

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "WSChat.WebSocketApi.dll"]
