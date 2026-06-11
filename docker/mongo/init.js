// =============================================
// PlataformaPedidos - Inicialización MongoDB
// =============================================

db = db.getSiblingDB('plataforma_pedidos');

db.createUser({
    user: 'pedidos_user',
    pwd: 'pedidos_pass',
    roles: [{ role: 'readWrite', db: 'plataforma_pedidos' }]
});

// Seed data: Clientes
db.clientes.insertMany([
    {
        _id: UUID('a1b2c3d4-e5f6-7890-abcd-ef1234567890'),
        Nombre: 'Cliente Demo 1'
    },
    {
        _id: UUID('b2c3d4e5-f6a7-8901-bcde-f12345678901'),
        Nombre: 'Cliente Demo 2'
    }
]);

// Seed data: Productos
db.productos.insertMany([
    {
        _id: UUID('c3d4e5f6-a7b8-9012-cdef-123456789012'),
        Nombre: 'Producto A',
        Descripcion: 'Descripción del Producto A',
        Precio: 100.50,
        FechaModificacion: new Date(),
        Disponible: true
    },
    {
        _id: UUID('d4e5f6a7-b8c9-0123-defa-234567890123'),
        Nombre: 'Producto B',
        Descripcion: 'Descripción del Producto B',
        Precio: 250.00,
        FechaModificacion: new Date(),
        Disponible: true
    },
    {
        _id: UUID('e5f6a7b8-c9d0-1234-efab-345678901234'),
        Nombre: 'Producto C',
        Descripcion: 'Descripción del Producto C',
        Precio: 75.99,
        FechaModificacion: new Date(),
        Disponible: false
    }
]);

// Seed data: Pedido de ejemplo
db.pedidos.insertOne({
    _id: UUID('f6a7b8c9-d0e1-2345-fabc-456789012345'),
    Cliente: {
        _id: UUID('a1b2c3d4-e5f6-7890-abcd-ef1234567890'),
        Nombre: 'Cliente Demo 1'
    },
    Detalles: [
        {
            Producto: {
                _id: UUID('c3d4e5f6-a7b8-9012-cdef-123456789012'),
                Nombre: 'Producto A',
                Descripcion: 'Descripción del Producto A',
                Precio: 100.50,
                FechaModificacion: new Date(),
                Disponible: true
            },
            Cantidad: 2,
            PrecioUnitario: 100.50,
            Subtotal: 201.00
        }
    ],
    Estado: 0,
    FechaCreacion: new Date(),
    FechaConfirmacion: null,
    Total: 201.00
});

print('Base de datos plataforma_pedidos inicializada correctamente.');
