cMaNGOS CMS - dotnet for API, nextjs for FE

Current capabilities:  
Registration  
Account Management  
2fa use/addition  
Basic world view of creatures and gameobjects for TBC  

How to setup:

Bare metal:

Install Visual Studio 2022 with .net 8  
Windows:  
Open and build using cmangos-web-api/cmangos-web-api.sln in Release mode  
Linux:  
TBD  
cmangos-web-api/cmangos-web-api/cmangos-web-api.csproj  

Generate HTTPS Certificate:  
Using dotnet:  
dotnet dev-certs https -ep aspnetapp.pfx -p crypticpassword  
Using OpenSSL:  
TBD  

Generate PEM keys for JWT:  
TBD  
Set them in AuthConfig:JwtPrivate and AuthConfig:JwtPublic fields in appsettings json  

Extract WOTLK DBCs using cmangos extractors  
Change cmangos-web-api/cmangos-web-api/appsettings.json field DbcConfig -> DirectoryPath to your path  

Install node 20 LTS  
inside cmangos-web-fe run `npm install`
run cmangos-web-fe/start.sh  

Docker:  

Install docker - on windows this includes installation of docker desktop and WSL, look up a different guide to do it  
Create certificates like on bare metal  
Create PEM keys like on bare metal  
Inside docker-compose.yaml change the following fields:  
Under volumes change - "YOUR_LOCAL_PATH:/opt/wotlk/dbc" to your path to wotlk dbcs  
If you have extracted https certificates to any other folder than where docker-compose.yaml is located change:  
ASPNETCORE_Kestrel__Certificates__Default__Path  
and  
ASPNETCORE_Kestrel__Certificates__Default__Password  
Also consult docker-compose.yaml if your DB config differs from cmangos defaults  
docker compose up  