#1. lépés
# ez az alapcsomag a telepítéshez
FROM microsoft/aspnetcore-build:2.0 as buildenv
WORKDIR /app

#az alkalmazás legfontosabb definícióit átmásoljuk az /app-ba
COPY rpi.server.csproj .
#lefuttatjuk a környezet helyreállítást
RUN dotnet restore ./rpi.server.csproj

#a teljes alkalmazást átmásoljuk az /app-ba
COPY . .
#elkészítjük az alkalmazást linux-arm architektúrára, és mellé csomagoljuk a .net
#környezetet is (-r linux-arm)
#Debug/Production
RUN dotnet publish -c Debug -o out -r linux-arm

#alkalmazás elkészült, alkalmas a linux-arm-en való futtatásra,
#külön dotnet telepítés nélkül

#2. lépés
#elkészítjük a docker csomagot
FROM microsoft/dotnet:2.0.0-runtime-deps-stretch-arm32v7
WORKDIR /app

COPY --from=buildenv /app/out .

#minden hálózati kapcsolat 5000-es portján szolgáltatunk
ENV ASPNETCORE_URLS=http://+:5000
#Development környezet amíg fejlesztünk
ENV ASPNETCORE_ENVIRONMENT=Development
#szólunk a dockernek, hogy az 5000-es portot engedje ki
EXPOSE 5000/tcp

#elindítjuk az alkalmazás
CMD ["./rpi.server"]
