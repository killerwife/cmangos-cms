Generation of PEM keys:
openssl genpkey -out rsakey.pem -algorithm RSA -pkeyopt rsa_keygen_bits:2048
openssl rsa -in rsakey.pem -pubout > rsakey-pub.pem

For development:

run cmangos-web-api.sln in Visual Studio 2022

For Docker:

Pick OS:
docker build -f Dockerfile.linux -t killerwife/cmangos-web-api:latest-linux .
docker build -f Dockerfile.windows -t killerwife/cmangos-web-api:latest-windows .

docker run -d -p 7191:80 killerwife/cmangos-web-fe:latest

Environment variables:

Consult docker compose corresponding to given OS
