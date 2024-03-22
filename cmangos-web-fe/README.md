## Getting Started

For development:

npm install

npm run dev

For deployment:

next build

next start

For docker use:

docker build -f Dockerfile -t killerwife/cmangos-web-fe:latest .
docker run -d -p 3000:3000 killerwife/cmangos-web-fe:latest