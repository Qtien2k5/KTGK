FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
RUN ln -s ~/.dotnet/tools/libman /usr/local/bin/libman

WORKDIR /src
COPY ["src/Ktgk.Web.Mvc/Ktgk.Web.Mvc.csproj", "src/Ktgk.Web.Mvc/"]
COPY ["src/Ktgk.Web.Core/Ktgk.Web.Core.csproj", "src/Ktgk.Web.Core/"]
COPY ["src/Ktgk.Application/Ktgk.Application.csproj", "src/Ktgk.Application/"]
COPY ["src/Ktgk.Core/Ktgk.Core.csproj", "src/Ktgk.Core/"]
COPY ["src/Ktgk.EntityFrameworkCore/Ktgk.EntityFrameworkCore.csproj", "src/Ktgk.EntityFrameworkCore/"]
WORKDIR "/src/src/Ktgk.Web.Mvc"
RUN dotnet restore 

WORKDIR /src
COPY ["src/Ktgk.Web.Mvc", "src/Ktgk.Web.Mvc"]
COPY ["src/Ktgk.Web.Core", "src/Ktgk.Web.Core"]
COPY ["src/Ktgk.Application", "src/Ktgk.Application"]
COPY ["src/Ktgk.Core", "src/Ktgk.Core"]
COPY ["src/Ktgk.EntityFrameworkCore", "src/Ktgk.EntityFrameworkCore"]
WORKDIR "/src/src/Ktgk.Web.Mvc"
RUN libman restore
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "Ktgk.Web.Mvc.dll"]
