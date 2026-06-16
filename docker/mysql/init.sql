-- =============================================
-- PlataformaPedidos - Inicialización MySQL
-- =============================================

CREATE TABLE IF NOT EXISTS Clientes (
    Id CHAR(36) PRIMARY KEY,
    Nombre VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS Productos (
    Id CHAR(36) PRIMARY KEY,
    Nombre VARCHAR(255) NOT NULL,
    Descripcion TEXT,
    Precio DECIMAL(18,2) NOT NULL,
    FechaModificacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    Disponible TINYINT(1) NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS Pedidos (
    Id CHAR(36) PRIMARY KEY,
    ClienteId CHAR(36) NOT NULL,
    Estado INT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FechaConfirmacion DATETIME NULL,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);

CREATE TABLE IF NOT EXISTS LineasPedido (
    PedidoId CHAR(36) NOT NULL,
    ProductoId CHAR(36) NOT NULL,
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
    ('c3d4e5f6-a7b8-9012-cdef-123456789012', 'Producto A', 'Descripción del Producto A', 100.50, 1),
    ('d4e5f6a7-b8c9-0123-defa-234567890123', 'Producto B', 'Descripción del Producto B', 250.00, 1),
    ('e5f6a7b8-c9d0-1234-efab-345678901234', 'Producto C', 'Descripción del Producto C', 75.99, 0);

-- Seed data: Pedido de ejemplo
INSERT INTO Pedidos (Id, ClienteId, Estado, FechaCreacion, Total) VALUES
    ('f6a7b8c9-d0e1-2345-fabc-456789012345', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 0, NOW(), 201.00);

INSERT INTO LineasPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario, Subtotal) VALUES
    ('f6a7b8c9-d0e1-2345-fabc-456789012345', 'c3d4e5f6-a7b8-9012-cdef-123456789012', 2, 100.50, 201.00);
