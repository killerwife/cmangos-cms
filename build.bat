cd cmangos-web-api

docker build -f Dockerfile.linux -t killerwife/cmangos-web-api:latest-linux .
docker push killerwife/cmangos-web-api:latest-linux

cd ../cmangos-web-fe

docker build -f Dockerfile -t killerwife/cmangos-web-fe:latest .
docker push killerwife/cmangos-web-fe:latest