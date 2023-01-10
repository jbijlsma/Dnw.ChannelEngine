FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy as build-image

RUN apt update
RUN apt install nodejs npm -y

WORKDIR /home/build

COPY ./*.sln ./
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done

RUN dotnet restore -r linux-musl-arm64

COPY . .

RUN dotnet publish ./Dnw.ChannelEngine.MerchantManager/Dnw.ChannelEngine.MerchantManager.csproj -o /publish/ 

FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy

WORKDIR /publish

COPY --from=build-image /publish .

ENV ASPNETCORE_URLS=http://+:5050
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Dnw.ChannelEngine.MerchantManager.dll"]