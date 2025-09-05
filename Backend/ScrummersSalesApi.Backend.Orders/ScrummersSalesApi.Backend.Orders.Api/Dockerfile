#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ScrummersSalesApi.Backend.Orders.Api/ScrummersSalesApi.Backend.Orders.Api.csproj", "ScrummersSalesApi.Backend.Orders.Api/"]
RUN dotnet restore "ScrummersSalesApi.Backend.Orders.Api/ScrummersSalesApi.Backend.Orders.Api.csproj"
COPY . .
WORKDIR "/src/ScrummersSalesApi.Backend.Orders.Api"
RUN dotnet build "ScrummersSalesApi.Backend.Orders.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScrummersSalesApi.Backend.Orders.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScrummersSalesApi.Backend.Orders.Api.dll"]