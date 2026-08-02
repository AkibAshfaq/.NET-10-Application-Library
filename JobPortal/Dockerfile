FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/JobPortal.Domain/*.csproj", "src/JobPortal.Domain/"]
COPY ["src/JobPortal.Application/*.csproj", "src/JobPortal.Application/"]
COPY ["src/JobPortal.Infrastructure/*.csproj", "src/JobPortal.Infrastructure/"]
COPY ["src/JobPortal.API/*.csproj", "src/JobPortal.API/"]
RUN dotnet restore "src/JobPortal.API/JobPortal.API.csproj"

COPY src/ src/
RUN dotnet publish "src/JobPortal.API/JobPortal.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "JobPortal.Api.dll"]