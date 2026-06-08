FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HeladeriaApi.csproj", "."]
RUN dotnet restore "HeladeriaApi.csproj"
COPY . .
RUN dotnet build "HeladeriaApi.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HeladeriaApi.dll"]