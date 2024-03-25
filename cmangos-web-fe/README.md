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

Environment variables: (.env as examples)

NEXT_PUBLIC_API - needs to face dotnet backend public exposed url
NEXT_PUBLIC_RECAPTCHA_SITE_KEY - public key part of recaptcha codes