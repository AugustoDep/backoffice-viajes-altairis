# Etapa 1: Construir Angular
FROM node:20-alpine AS build
WORKDIR /app

# Copiar archivos de configuración
COPY package*.json ./
RUN npm ci --legacy-peer-deps

# Copiar el código fuente
COPY . .

# Construir la aplicación
RUN npm run build 

# Etapa 2: Servir con Nginx
FROM nginx:alpine

# Copiar los archivos construidos
COPY --from=build /app/dist/backoffice-altairis-web-app/browser /usr/share/nginx/html

# Copiar configuración de nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]