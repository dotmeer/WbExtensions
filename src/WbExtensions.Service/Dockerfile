FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

EXPOSE 8000

COPY . .

WORKDIR /WbExtensions.Service
RUN dotnet restore "WbExtensions.Service.csproj"
RUN dotnet build "WbExtensions.Service.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "WbExtensions.Service.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WbExtensions.Service.dll"]
