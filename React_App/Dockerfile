FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
ARG VITE_GATEWAY_URL=http://localhost:5050
ARG VITE_API_TOKEN_URL=http://localhost:5010
ARG VITE_CALLBACK_URL=http://localhost:3000/login-callback
ARG VITE_APP_ID=react-app
ENV VITE_GATEWAY_URL=$VITE_GATEWAY_URL
ENV VITE_API_TOKEN_URL=$VITE_API_TOKEN_URL
ENV VITE_CALLBACK_URL=$VITE_CALLBACK_URL
ENV VITE_APP_ID=$VITE_APP_ID
RUN npm run build

FROM nginx:alpine AS final
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
