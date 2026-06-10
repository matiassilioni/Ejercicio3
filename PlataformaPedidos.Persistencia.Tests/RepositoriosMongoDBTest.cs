using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PlataformaPedidos.Dominio.Enumeraciones;
using PlataformaPedidos.Dominio.Modelos;
using PlataformaPedidos.Persistencia.Mongodb;

namespace PlataformaPedidos.Persistencia.Tests;

public class RepositoriosMongoDBTest
{
    private const string ConnectionString =
        "mongodb://pedidos_user:pedidos_pass@localhost:27018/plataforma_pedidos";
    private const string DatabaseName = "plataforma_pedidos";

    private static MongoDbContext CreateContext() => new(ConnectionString, DatabaseName);

    private static readonly Guid ClienteDemo1Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    private static readonly Guid ClienteDemo2Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901");
    private static readonly Guid ProductoAId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012");
    private static readonly Guid ProductoBId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123");
    private static readonly Guid PedidoSeedId = Guid.Parse("f6a7b8c9-d0e1-2345-fabc-456789012345");

    // ========== CLIENTES ==========

    [Fact]
    public void GetCliente_ConIdExistente_RetornaCliente()
    {
        var context = CreateContext();
        var repo = new RepositorioClientes(context, NullLogger<RepositorioClientes>.Instance);

        var cliente = repo.GetCliente(ClienteDemo1Id);

        Assert.NotNull(cliente);
        Assert.Equal("Cliente Demo 1", cliente.Nombre);
    }

    [Fact]
    public void SaveOrUpdateCliente_NuevoCliente_CreaCliente()
    {
        var context = CreateContext();
        var repo = new RepositorioClientes(context, NullLogger<RepositorioClientes>.Instance);
        var nuevoId = Guid.NewGuid();

        var resultado = repo.SaveOrUpdateCliente(new Cliente { Id = nuevoId, Nombre = "Cliente Test MongoDB" });

        Assert.True(resultado);
        var guardado = repo.GetCliente(nuevoId);
        Assert.NotNull(guardado);
        Assert.Equal("Cliente Test MongoDB", guardado.Nombre);
    }

    [Fact]
    public void SaveOrUpdateCliente_ClienteExistente_ActualizaCliente()
    {
        var context = CreateContext();
        var repo = new RepositorioClientes(context, NullLogger<RepositorioClientes>.Instance);
        const string nombreActualizado = "Cliente Demo 2 Actualizado";

        var resultado = repo.SaveOrUpdateCliente(new Cliente { Id = ClienteDemo2Id, Nombre = nombreActualizado });

        Assert.True(resultado);
        var guardado = repo.GetCliente(ClienteDemo2Id);
        Assert.NotNull(guardado);
        Assert.Equal(nombreActualizado, guardado.Nombre);

        repo.SaveOrUpdateCliente(new Cliente { Id = ClienteDemo2Id, Nombre = "Cliente Demo 2" });
    }

    // ========== PEDIDOS ==========

    [Fact]
    public void GetById_ConIdExistente_RetornaPedidoConClienteYDetalles()
    {
        var context = CreateContext();
        var repo = new RepositorioPedidos(context, NullLogger<RepositorioPedidos>.Instance);

        var pedido = repo.GetById(PedidoSeedId);

        Assert.NotNull(pedido);
        Assert.NotNull(pedido.Cliente);
        Assert.NotEmpty(pedido.Detalles);
        Assert.Equal(EstadoPedido.Pendiente, pedido.Estado);
    }

    [Fact]
    public void GetAll_RetornaListaPedidos()
    {
        var context = CreateContext();
        var repo = new RepositorioPedidos(context, NullLogger<RepositorioPedidos>.Instance);

        var pedidos = repo.GetAll();

        Assert.NotNull(pedidos);
        Assert.NotEmpty(pedidos);
    }

    [Fact]
    public void SaveOrUpdate_NuevoPedido_CreaPedido()
    {
        var context = CreateContext();
        var repoCliente = new RepositorioClientes(context, NullLogger<RepositorioClientes>.Instance);
        var repoProducto = new RepositorioProductos(context, NullLogger<RepositorioProductos>.Instance);
        var repoPedido = new RepositorioPedidos(context, NullLogger<RepositorioPedidos>.Instance);

        var cliente = repoCliente.GetCliente(ClienteDemo1Id);
        var producto = repoProducto.GetById(ProductoAId);
        var nuevoId = Guid.NewGuid();

        var pedido = new Pedido
        {
            Id = nuevoId,
            Cliente = cliente,
            Estado = EstadoPedido.Pendiente,
            FechaCreacion = DateTime.UtcNow,
            Total = producto.Precio * 2,
            Detalles = new List<LineaPedido>
            {
                new()
                {
                    Producto = producto,
                    Cantidad = 2,
                    PrecioUnitario = producto.Precio,
                    Subtotal = producto.Precio * 2
                }
            }
        };

        var resultado = repoPedido.SaveOrUpdate(pedido);

        Assert.True(resultado);
        var guardado = repoPedido.GetById(nuevoId);
        Assert.NotNull(guardado);
        Assert.Equal(EstadoPedido.Pendiente, guardado.Estado);
        Assert.Single(guardado.Detalles);
    }

    // ========== PRODUCTOS ==========

    [Fact]
    public void GetById_ProductoExistente_RetornaProducto()
    {
        var context = CreateContext();
        var repo = new RepositorioProductos(context, NullLogger<RepositorioProductos>.Instance);

        var producto = repo.GetById(ProductoAId);

        Assert.NotNull(producto);
        Assert.Equal("Producto A", producto.Nombre);
    }

    [Fact]
    public void GetAll_Productos_RetornaLista()
    {
        var context = CreateContext();
        var repo = new RepositorioProductos(context, NullLogger<RepositorioProductos>.Instance);

        var productos = repo.GetAll();

        Assert.NotNull(productos);
        Assert.NotEmpty(productos);
    }

    [Fact]
    public void SaveOrUpdateProducto_NuevoProducto_CreaProducto()
    {
        var context = CreateContext();
        var repo = new RepositorioProductos(context, NullLogger<RepositorioProductos>.Instance);
        var nuevoId = Guid.NewGuid();

        var nuevo = new Producto
        {
            Id = nuevoId,
            Nombre = "Producto Test MongoDB",
            Descripcion = "Test",
            Precio = 99.99m,
            FechaModificacion = DateTime.UtcNow,
            Disponible = true
        };

        var resultado = repo.SaveOrUpdateProducto(nuevo);

        Assert.True(resultado);
        var guardado = repo.GetById(nuevoId);
        Assert.NotNull(guardado);
        Assert.Equal("Producto Test MongoDB", guardado.Nombre);
    }

    [Fact]
    public void SaveOrUpdateProducto_ProductoExistente_ActualizaProducto()
    {
        var context = CreateContext();
        var repo = new RepositorioProductos(context, NullLogger<RepositorioProductos>.Instance);
        var productoActualizado = new Producto
        {
            Id = ProductoBId,
            Nombre = "Producto B Actualizado",
            Descripcion = "Actualizado",
            Precio = 300.00m,
            FechaModificacion = DateTime.UtcNow,
            Disponible = false
        };

        var resultado = repo.SaveOrUpdateProducto(productoActualizado);

        Assert.True(resultado);
        var guardado = repo.GetById(ProductoBId);
        Assert.NotNull(guardado);
        Assert.Equal("Producto B Actualizado", guardado.Nombre);

        repo.SaveOrUpdateProducto(new Producto
        {
            Id = ProductoBId,
            Nombre = "Producto B",
            Descripcion = "Descripci\u00f3n del Producto B",
            Precio = 250.00m,
            FechaModificacion = DateTime.UtcNow,
            Disponible = true
        });
    }
}
