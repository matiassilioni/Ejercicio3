-- =============================================
-- PlataformaPedidos - Inicialización PostgreSQL
-- =============================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS Clientes (
    Id UUID PRIMARY KEY,
    Nombre VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS Productos (
    Id UUID PRIMARY KEY,
    Nombre VARCHAR(255) NOT NULL,
    Descripcion TEXT,
    Precio DECIMAL(18,2) NOT NULL,
    FechaModificacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Disponible BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS Pedidos (
    Id UUID PRIMARY KEY,
    ClienteId UUID NOT NULL,
    Estado INT NOT NULL DEFAULT 0,
    FechaCreacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FechaConfirmacion TIMESTAMP NULL,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);

CREATE TABLE IF NOT EXISTS LineasPedido (
    PedidoId UUID NOT NULL,
    ProductoId UUID NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (PedidoId, ProductoId),
    FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id),
    FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);

-- Seed data
INSERT INTO Clientes (Id, Nombre) VALUES
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Cliente Demo 1'),
    ('b2c3d4e5-f6a7-8901-bcde-f12345678901', 'Cliente Demo 2');

INSERT INTO Productos (Id, Nombre, Descripcion, Precio, Disponible) VALUES
    ('c3d4e5f6-a7b8-9012-cdef-123456789012', 'Producto A', 'Descripción del Producto A', 100.50, TRUE),
    ('d4e5f6a7-b8c9-0123-defa-234567890123', 'Producto B', 'Descripción del Producto B', 250.00, TRUE),
    ('e5f6a7b8-c9d0-1234-efab-345678901234', 'Producto C', 'Descripción del Producto C', 75.99, FALSE);
