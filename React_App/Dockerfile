FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
ARG VITE_AUTH_AUTHORITY=http://localhost:5010
ARG VITE_CLIENT_ID=react-app
ARG VITE_REDIRECT_URI=http://localhost:3000/callback
ARG VITE_POST_LOGOUT_URI=http://localhost:3000
ARG VITE_GATEWAY_URL=http://localhost:5050
ENV VITE_AUTH_AUTHORITY=$VITE_AUTH_AUTHORITY
ENV VITE_CLIENT_ID=$VITE_CLIENT_ID
ENV VITE_REDIRECT_URI=$VITE_REDIRECT_URI
ENV VITE_POST_LOGOUT_URI=$VITE_POST_LOGOUT_URI
ENV VITE_GATEWAY_URL=$VITE_GATEWAY_URL
RUN npm run build

FROM nginx:alpine AS final
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
