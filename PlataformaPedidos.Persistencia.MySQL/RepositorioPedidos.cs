using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.MySQL;

public class RepositorioPedidos : IRepositorioPedidos
{
    private readonly MySqlConnection _connection;
    private readonly ILogger<RepositorioPedidos> _logger;

    public RepositorioPedidos(MySqlConnection connection, ILogger<RepositorioPedidos> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public Pedido GetById(Guid id)
    {
        try
        {
            var pedido = GetPedidoBase(_connection, id);
            if (pedido == null) return null;

            pedido.Cliente = GetClienteDePedido(_connection, id);
            pedido.Detalles = GetLineasDePedido(_connection, id);

            return pedido;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pedido con Id {PedidoId}", id);
            return null;
        }
    }

    public List<Pedido> GetAll()
    {
        try
        {
            const string sqlPedidos = @"
                SELECT Id, ClienteId, Estado, FechaCreacion, FechaConfirmacion, Total
                FROM Pedidos";

            var pedidos = _connection.Query<Pedido>(sqlPedidos).AsList();

            if (pedidos.Count == 0) return pedidos;

            var ids = pedidos.Select(p => p.Id).ToList();

            var clientes = _connection.Query<Cliente>(@"
                SELECT DISTINCT c.Id, c.Nombre
                FROM Clientes c
                INNER JOIN Pedidos p ON p.ClienteId = c.Id
                WHERE p.Id IN @Ids", new { Ids = ids })
                .ToDictionary(c => c.Id);

            var lineasRaw = _connection.Query(@"
                SELECT lp.PedidoId, lp.Cantidad, lp.PrecioUnitario, lp.Subtotal,
                       p.Id AS ProductoId, p.Nombre, p.Descripcion, p.Precio,
                       p.FechaModificacion, p.Disponible
                FROM LineasPedido lp
                INNER JOIN Productos p ON p.Id = lp.ProductoId
                WHERE lp.PedidoId IN @Ids", new { Ids = ids })
                .GroupBy(row => (Guid)row.PedidoId)
                .ToDictionary(g => g.Key, g => g.Select(row => new LineaPedido
                {
                    Cantidad = (int)row.Cantidad,
                    PrecioUnitario = (decimal)row.PrecioUnitario,
                    Subtotal = (decimal)row.Subtotal,
                    Producto = new Producto
                    {
                        Id = (Guid)row.ProductoId,
                        Nombre = (string)row.Nombre,
                        Descripcion = (string)row.Descripcion,
                        Precio = (decimal)row.Precio,
                        FechaModificacion = (DateTime)row.FechaModificacion,
                        Disponible = (bool)row.Disponible
                    }
                }).ToList());

            foreach (var pedido in pedidos)
            {
                if (clientes.TryGetValue(pedido.Id, out var cliente))
                    pedido.Cliente = cliente;

                if (lineasRaw.TryGetValue(pedido.Id, out var detalles))
                    pedido.Detalles = detalles;
            }

            return pedidos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los pedidos");
            return new List<Pedido>();
        }
    }

    public bool SaveOrUpdate(Pedido pedido)
    {
        try
        {
            using var transaction = _connection.BeginTransaction();

            try
            {
                const string sqlPedido = @"
                    INSERT INTO Pedidos (Id, ClienteId, Estado, FechaCreacion, FechaConfirmacion, Total)
                    VALUES (@Id, @ClienteId, @Estado, @FechaCreacion, @FechaConfirmacion, @Total)
                    ON DUPLICATE KEY UPDATE
                        ClienteId = @ClienteId,
                        Estado = @Estado,
                        FechaCreacion = @FechaCreacion,
                        FechaConfirmacion = @FechaConfirmacion,
                        Total = @Total";

                _connection.Execute(sqlPedido, new
                {
                    pedido.Id,
                    ClienteId = pedido.Cliente?.Id,
                    pedido.Estado,
                    pedido.FechaCreacion,
                    pedido.FechaConfirmacion,
                    pedido.Total
                }, transaction);

                _connection.Execute(
                    "DELETE FROM LineasPedido WHERE PedidoId = @PedidoId",
                    new { PedidoId = pedido.Id },
                    transaction);

                if (pedido.Detalles != null && pedido.Detalles.Count > 0)
                {
                    const string sqlLineas = @"
                        INSERT INTO LineasPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario, Subtotal)
                        VALUES (@PedidoId, @ProductoId, @Cantidad, @PrecioUnitario, @Subtotal)";

                    var lineas = pedido.Detalles.Select(l => new
                    {
                        PedidoId = pedido.Id,
                        ProductoId = l.Producto?.Id,
                        l.Cantidad,
                        l.PrecioUnitario,
                        l.Subtotal
                    });

                    _connection.Execute(sqlLineas, lineas, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar pedido con Id {PedidoId}", pedido.Id);
            return false;
        }
    }

    private static Pedido GetPedidoBase(MySqlConnection connection, Guid id)
    {
        const string sql = @"
            SELECT Id, ClienteId, Estado, FechaCreacion, FechaConfirmacion, Total
            FROM Pedidos WHERE Id = @Id";
        return connection.QuerySingleOrDefault<Pedido>(sql, new { Id = id });
    }

    private static Cliente GetClienteDePedido(MySqlConnection connection, Guid pedidoId)
    {
        const string sql = @"
            SELECT c.Id, c.Nombre
            FROM Clientes c
            INNER JOIN Pedidos p ON p.ClienteId = c.Id
            WHERE p.Id = @PedidoId";
        return connection.QuerySingleOrDefault<Cliente>(sql, new { PedidoId = pedidoId });
    }

    private List<LineaPedido> GetLineasDePedido(MySqlConnection connection, Guid pedidoId)
    {
        const string sql = @"
            SELECT lp.Cantidad, lp.PrecioUnitario, lp.Subtotal,
                   p.Id, p.Nombre, p.Descripcion, p.Precio, p.FechaModificacion, p.Disponible
            FROM LineasPedido lp
            INNER JOIN Productos p ON p.Id = lp.ProductoId
            WHERE lp.PedidoId = @PedidoId";

        return connection.Query<LineaPedido, Producto, LineaPedido>(
            sql,
            (linea, producto) =>
            {
                linea.Producto = producto;
                return linea;
            },
            new { PedidoId = pedidoId },
            splitOn: "Id").AsList();
    }
}
