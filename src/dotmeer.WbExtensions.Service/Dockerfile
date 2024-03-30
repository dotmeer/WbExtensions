FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

EXPOSE 8000

COPY . .

WORKDIR /dotmeer.WbExtensions.Service
RUN dotnet restore "dotmeer.WbExtensions.Service.csproj"
RUN dotnet build "dotmeer.WbExtensions.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dotmeer.WbExtensions.Service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotmeer.WbExtensions.Service.dll"]
