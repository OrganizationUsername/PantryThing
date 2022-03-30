#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Pantry.WebApi/Pantry.WebApi.csproj", "Pantry.WebApi/"]
RUN dotnet restore "Pantry.WebApi/Pantry.WebApi.csproj"
COPY . .
WORKDIR "/src/Pantry.WebApi"
RUN dotnet build "Pantry.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pantry.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pantry.WebApi.dll"]