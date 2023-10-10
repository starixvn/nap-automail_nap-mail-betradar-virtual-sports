FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /src

COPY ./Fanex.BetradarVirtualSports.Service/Fanex.BetradarVirtualSports.Business ./Fanex.BetradarVirtualSports.Business
COPY ./Fanex.BetradarVirtualSports.Service/Fanex.BetradarVirtualSports.Service/Fanex.BetradarVirtualSports.Service.csproj ./Fanex.BetradarVirtualSports.Service/Fanex.BetradarVirtualSports.Service.csproj

RUN ls /src -l
WORKDIR /src/Fanex.BetradarVirtualSports.Service
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages
RUN dotnet restore -v n

COPY ./Fanex.BetradarVirtualSports.Service/Fanex.BetradarVirtualSports.Service .

RUN --mount=type=cache,target=/root/.nuget/packages
RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/rebuild-baseimage/dotnetcore/aspnet:7.0-nap-mail-core



# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

ENV TZ="America/La_Paz"
ENV COMPlus_EnableDiagnostics=0

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.BetradarVirtualSports.Service.dll"]
