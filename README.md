# Hotel Booking System

Sistema de gestión de reservas de hoteles desarrollado con **.NET 8**, **Angular 19** y **MySQL**. La aplicación permite administrar hoteles, habitaciones y reservas, con un dashboard de disponibilidad y un sistema completo de logs de actividades.

---

## 🚀 Tecnologías Utilizadas

| Tecnología | Versión | Descripción |
|------------|---------|-------------|
| **Backend** | .NET 8 | API RESTful con Entity Framework Core |
| **Frontend** | Angular 19 | SPA con componentes standalone y signals |
| **Base de Datos** | MySQL 8.0 | Base de datos relacional |
| **Contenedores** | Docker & Docker Compose | Orquestación de servicios |
| **Servidor Web** | Nginx | Servidor para Angular en producción |

---

## 📋 Prerrequisitos

Antes de comenzar, asegúrate de tener instalado:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (versión 24.0 o superior)
- [Git](https://git-scm.com/) (opcional, para clonar el repositorio)

> **Nota:** No es necesario tener instalado .NET SDK, Node.js o MySQL localmente. Docker se encarga de todo.

---

## 🚀 Levantar la Aplicación

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/backoffice-viajes-altairis.git
cd backoffice-viajes-altairis ```
```

### 2. Levantar todos los servicios

```bash
docker-compose up -d
```

Este comando:
Descarga las imágenes necesarias (MySQL, .NET SDK, Node.js, Nginx)
Construye las imágenes del backend y frontend
Crea y levanta los contenedores
Aplica las migraciones de base de datos automáticamente

### 3. Verificar que todo está funcionando

```bash
docker-compose ps
```

Deberías ver los tres contenedores con estado Up:

text
NAME                   IMAGE                              STATUS
hotelbooking-mysql     mysql:8.0                          Up
hotelbooking-backend   backoffice-viajes-altairis-backend Up
hotelbooking-frontend  backoffice-viajes-altairis-frontend Up

### 4. Acceder a la aplicación
Servicio	URL
Frontend	http://localhost:4200
Backend API	http://localhost:5120/api/hotels
Base de Datos	localhost:3307 (usuario: root, password: root)

### 📊 Datos de Prueba
La aplicación incluye datos de prueba precargados
10 hoteles
Más de 30 habitaciones
Reservas de ejemplo
Puedes ver los hoteles directamente en la pantalla principal del frontend.

### 🛠️ Comandos Útiles

docker-compose up -d	Levantar todos los servicios en segundo plano
docker-compose down	Detener todos los servicios
docker-compose down -v	Detener y eliminar volúmenes (borra datos)
docker-compose logs -f	Ver logs en tiempo real
docker-compose logs backend	Ver logs solo del backend
docker-compose build backend --no-cache	Reconstruir el backend sin caché
docker-compose build frontend --no-cache	Reconstruir el frontend sin caché

### 📁 Estructura del Proyecto

backoffice-viajes-altairis/
├── docker-compose.yml          # Orquestación de servicios
├── .gitignore                  # Archivos ignorados
├── README.md                   # Este archivo
├── database/
│   └── seed-data.sql           # Datos de prueba
├── BackofficeAltairis/         # Backend .NET
│   ├── Dockerfile
│   ├── Program.cs
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   └── Data/
└── backoffice-altairis-web-app/ # Frontend Angular
    ├── Dockerfile
    ├── nginx.conf
    ├── src/
    └── package.json

### 🔧 Configuración de Puertos
Servicio	Puerto Interno	Puerto Externo
MySQL	3306	3307
Backend	8080	5120
Frontend	80	4200
Si algún puerto está ocupado, puedes modificarlos en docker-compose.yml.

🐛 Solución de Problemas
Error: Puerto 3306 ocupado
bash
# Cambiar el puerto en docker-compose.yml
ports:
  - "3308:3306"  # Cambiar 3307 por otro puerto
Error: El frontend no carga
bash
# Reconstruir el frontend
docker-compose build frontend --no-cache
docker-compose up -d frontend
Error: El backend no conecta a MySQL
bash
# Verificar logs del backend
docker logs hotelbooking-backend --tail 50
Verificar que los contenedores están corriendo
bash
docker ps
docker logs hotelbooking-backend
docker logs hotelbooking-mysql
🧪 Probar la API manualmente
bash
# Obtener todos los hoteles
curl http://localhost:5120/api/hotels?page=1&pageSize=25

# Obtener un hotel específico
curl http://localhost:5120/api/hotels/1

# Ver habitaciones de un hotel
curl http://localhost:5120/api/hotels/1/rooms

# Ver disponibilidad
curl "http://localhost:5120/api/availability/rooms?hotelId=1&checkIn=2026-04-01&checkOut=2026-04-05"

### 📝 Características Principales
✅ CRUD completo de hoteles y habitaciones

✅ Sistema de reservas por rango de fechas

✅ Dashboard de disponibilidad con gráficos mensuales

✅ Filtros por país, ciudad y calificación de estrellas

✅ Logs de actividades (creación, edición, reservas)

✅ Paginación en el listado de hoteles

✅ Dockerizado para despliegue inmediato


### 📄 Licencia
Este proyecto es de uso educativo y demostrativo.

### ✨ Demo
Una vez levantada la aplicación, puedes:

Ver hoteles en http://localhost:4200
Agregar hoteles desde el botón "Add New Hotel"
Ver detalles haciendo clic en "View Details"
Reservar habitaciones seleccionando fechas y haciendo clic en "Book Now"
Ver dashboard de disponibilidad en la pestaña "Availability"
Auditar logs en la pestaña "Logs"

### 🐳 Comando Único
Para levantar toda la aplicación en una máquina nueva:

```bash
git clone <repositorio> && cd <carpeta> && docker-compose up -d
```

¡Listo! La aplicación estará funcionando en menos de 5 minutos.

