#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see http://aka.ms/containercompat 

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY SpotHero.Api/SpotHero.Api.csproj SpotHero.Api/
COPY SpotHero.Operations/SpotHero.Operations.csproj SpotHero.Operations/
COPY SpotHero.DataModel/SpotHero.DataModel.csproj SpotHero.DataModel/
COPY SpotHero.DataAccess/SpotHero.DataAccess.csproj SpotHero.DataAccess/
COPY SpotHero.Common/SpotHero.Common.csproj SpotHero.Common/
RUN dotnet restore SpotHero.Api/SpotHero.Api.csproj
COPY . .
WORKDIR /src/SpotHero.Api
RUN dotnet build SpotHero.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish SpotHero.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SpotHero.Api.dll"]
