# Docker Compose - Bases de Datos PlataformaPedidos

Este proyecto contiene la configuración de Docker Compose para levantar las bases de datos que utiliza la aplicación **PlataformaPedidos**: MySQL, PostgreSQL y MongoDB.

---

## Requisitos

- Tener **Docker Desktop** instalado y ejecutándose.
  - Si no lo tienes, descárgalo desde [docker.com](https://www.docker.com/products/docker-desktop/).

---

## Cómo levantar los contenedores

### 1. Abre una terminal

Abre **PowerShell**, **CMD** o cualquier terminal que tengas instalada.

### 2. Navega hasta la carpeta `docker`

```bash
cd ruta/del/proyecto/docker
```

> *Reemplaza `ruta/del/proyecto` con la ruta real donde está el proyecto en tu computadora.*

### 3. Levanta los contenedores

```bash
docker compose up -d
```

Este comando descarga las imágenes (la primera vez puede tardar unos minutos) y levanta los tres contenedores en segundo plano.

- El flag `-d` hace que los contenedores corran en background (no ves los logs en la terminal).

### 4. Verifica que todo esté funcionando

```bash
docker compose ps
```

Deberías ver los tres servicios con estado **"Up"** (en ejecución).

También puedes ver los logs con:

```bash
docker compose logs -f
```

Presiona `Ctrl + C` para salir de los logs.

---

## Detalles de conexión

Cada base de datos queda disponible en los siguientes puertos:

| Base de Datos | Puerto (host) | Puerto (contenedor) | Usuario     | Contraseña     | Base de Datos       |
|---------------|---------------|---------------------|-------------|----------------|---------------------|
| MySQL         | `3307`        | `3306`              | `pedidos_user` | `pedidos_pass` | `plataforma_pedidos` |
| PostgreSQL    | `5433`        | `5432`              | `pedidos_user` | `pedidos_pass` | `plataforma_pedidos` |
| MongoDB       | `27018`       | `27017`             | `pedidos_user` | `pedidos_pass` | `plataforma_pedidos` |

> **Nota:** Se usan puertos distintos a los default (`3307` en vez de `3306`, `5433` en vez de `5432`, `27018` en vez de `27017`) para evitar conflictos si ya tienes esos servicios instalados en tu máquina.

### Ejemplos de connection strings

**MySQL:**
```
Server=localhost;Port=3307;Database=plataforma_pedidos;User Id=pedidos_user;Password=pedidos_pass;
```

**PostgreSQL:**
```
Host=localhost;Port=5433;Database=plataforma_pedidos;Username=pedidos_user;Password=pedidos_pass;
```

**MongoDB:**
```
mongodb://pedidos_user:pedidos_pass@localhost:27018/plataforma_pedidos
```

---

## Datos de prueba (seed)

Los scripts de inicialización agregan automáticamente datos de ejemplo al levantar los contenedores por primera vez:

- **2 clientes** de demostración
- **3 productos** (2 disponibles, 1 no disponible)
- En MongoDB se agrega adicionalmente **1 pedido de ejemplo** con una línea de pedido

Si quieres empezar desde cero, borra los volúmenes (esto elimina **todos los datos**):

```bash
docker compose down -v
```

Luego vuelve a levantar con `docker compose up -d`.

---

## Comandos útiles

| Acción                                 | Comando                        |
|----------------------------------------|--------------------------------|
| Levantar contenedores                  | `docker compose up -d`        |
| Detener contenedores                   | `docker compose down`         |
| Detener y borrar datos (volúmenes)     | `docker compose down -v`      |
| Ver logs en tiempo real                | `docker compose logs -f`      |
| Ver estado de los contenedores         | `docker compose ps`           |
| Reiniciar un servicio específico       | `docker compose restart mysql` |

---

## Notas importantes

- Los datos persisten entre reinicios gracias a los **volúmenes** de Docker (`mysql_data`, `postgres_data`, `mongo_data`).
- Si cambias los scripts de inicialización (`init.sql` / `init.js`), deberás borrar los volúmenes (`docker compose down -v`) y volver a levantar para que se ejecuten de nuevo.
- La primera vez que ejecutes `docker compose up` se descargarán las imágenes de Docker Hub. Esto puede demorar dependiendo de tu conexión a internet.
