# Stress Test — PlataformaPedidos

Script de stress test con [k6](https://k6.io/) para la API `PlataformaPedidos.Api`.

## Requisitos

- **k6** instalado (ver [Instalación](#instalacin) abajo)
- API corriendo en `http://localhost:5257`
- Base de datos inicializada con seed data (ejecutar `docker-compose up` desde la carpeta `docker/`)

## Instalación de k6

### Windows (con winget)
```powershell
winget install k6
```

### Windows (manual)
1. Descargar el binario desde https://k6.io/docs/getting-started/installation/
2. Extraer `k6.exe` y agregarlo al PATH

### macOS
```bash
brew install k6
```

### Linux (Debian/Ubuntu)
```bash
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update && sudo apt-get install k6
```

Verificar instalación:
```bash
k6 version
```

## Endpoints que ejerce el test

| Acción | Método | URL |
|---|---|---|
| Crear cliente | POST | `/api/clientes` |
| Crear pedido | POST | `/api/pedidos` |
| Confirmar pedido | PUT | `/api/pedidos/{id}/confirmar` |
| Cancelar pedido | PUT | `/api/pedidos/{id}/cancelar` |

## Flujo

Por cada iteración de VU:

1. Crea `N` clientes (`POST /api/clientes`)
2. Por cada cliente, crea `M` pedidos (`POST /api/pedidos`) usando el producto `c3d4e5f6-a7b8-9012-cdef-123456789012`
3. Intercala confirmación/cancelación: pedidos pares se confirman, impares se cancelan

## Variables de entorno

| Variable | Default | Descripción |
|---|---|---|
| `N` | `2` | Clientes por VU |
| `M` | `4` | Pedidos por cliente |
| `TARGET` | `5` | Cantidad de VUs en el plateau |
| `DURATION` | `2m` | Duración del plateau |

## Ejecución

```bash
# Valores por defecto (2 clientes, 4 pedidos c/u, 5 VUs, 2 min)
k6 run stress-test.js

# Personalizado: 3 clientes, 6 pedidos c/u, 10 VUs, 5 minutos
k6 run --env N=3 --env M=6 --env TARGET=10 --env DURATION=5m stress-test.js
```

## Verificación previa

Antes de ejecutar el stress test, verificar que el producto de prueba existe:

```bash
curl http://localhost:5257/api/productos
```

Debe aparecer el producto con ID `c3d4e5f6-a7b8-9012-cdef-123456789012` (Producto A, $100.50).

## Limpieza

El test genera datos nuevos en cada ejecución. Para limpiar la base de datos:

```sql
TRUNCATE TABLE LineasPedido;
TRUNCATE TABLE Pedidos;
-- Clientes creados por k6 se pueden borrar por nombre (contienen "K6-")
DELETE FROM Clientes WHERE Nombre LIKE 'K6-%';
```
