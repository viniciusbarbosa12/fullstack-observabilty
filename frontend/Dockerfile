FROM node:18-alpine AS build
WORKDIR /app

COPY . .

RUN npm install
RUN npm run build  # Isso vai gerar a pasta build

FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html  

EXPOSE 80
