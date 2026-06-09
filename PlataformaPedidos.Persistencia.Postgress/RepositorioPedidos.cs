using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.Postgress;

public class RepositorioPedidos : IRepositorioPedidos
{
    private readonly IDbConnection _connection;
    private readonly ILogger<RepositorioPedidos> _logger;

    public RepositorioPedidos(IDbConnection connection, ILogger<RepositorioPedidos> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public Pedido GetById(Guid id)
    {
        try
        {
            var pedido = _connection.Query<Pedido, Cliente, Pedido>(
                @"SELECT p.Id, p.Estado, p.FechaCreacion, p.FechaConfirmacion, p.Total,
                         c.Id, c.Nombre
                  FROM Pedidos p
                  INNER JOIN Clientes c ON c.Id = p.ClienteId
                  WHERE p.Id = @Id",
                (pedido, cliente) =>
                {
                    pedido.Cliente = cliente;
                    return pedido;
                },
                new { Id = id },
                splitOn: "Id").FirstOrDefault();

            if (pedido == null) return null;

            var lineas = _connection.Query<LineaPedido, Producto, LineaPedido>(
                @"SELECT lp.Cantidad, lp.PrecioUnitario, lp.Subtotal,
                         pr.Id, pr.Nombre, pr.Descripcion, pr.Precio, pr.FechaModificacion, pr.Disponible
                  FROM LineasPedido lp
                  INNER JOIN Productos pr ON pr.Id = lp.ProductoId
                  WHERE lp.PedidoId = @PedidoId",
                (linea, producto) =>
                {
                    linea.Producto = producto;
                    return linea;
                },
                new { PedidoId = id },
                splitOn: "Id").AsList();

            pedido.Detalles = lineas;
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
            var pedidoDict = new Dictionary<Guid, Pedido>();

            _connection.Query<Pedido, Cliente, Pedido>(
                @"SELECT p.Id, p.Estado, p.FechaCreacion, p.FechaConfirmacion, p.Total,
                         c.Id, c.Nombre
                  FROM Pedidos p
                  INNER JOIN Clientes c ON c.Id = p.ClienteId",
                (pedido, cliente) =>
                {
                    if (!pedidoDict.TryGetValue(pedido.Id, out var existing))
                    {
                        pedido.Cliente = cliente;
                        pedido.Detalles = new List<LineaPedido>();
                        pedidoDict[pedido.Id] = pedido;
                    }
                    return pedido;
                },
                splitOn: "Id");

            if (pedidoDict.Count == 0) return new List<Pedido>();

            var lineasData = _connection.Query(
                @"SELECT lp.PedidoId, lp.Cantidad, lp.PrecioUnitario, lp.Subtotal,
                         pr.Id, pr.Nombre, pr.Descripcion, pr.Precio, pr.FechaModificacion, pr.Disponible
                  FROM LineasPedido lp
                  INNER JOIN Productos pr ON pr.Id = lp.ProductoId
                  WHERE lp.PedidoId = ANY(@Ids)",
                new { Ids = pedidoDict.Keys.ToList() }).ToList();

            foreach (dynamic row in lineasData)
            {
                Guid pedidoId = row.PedidoId;
                if (pedidoDict.TryGetValue(pedidoId, out var pedido))
                {
                    pedido.Detalles.Add(new LineaPedido
                    {
                        Producto = new Producto
                        {
                            Id = row.Id,
                            Nombre = row.Nombre,
                            Descripcion = row.Descripcion,
                            Precio = row.Precio,
                            FechaModificacion = row.FechaModificacion,
                            Disponible = row.Disponible
                        },
                        Cantidad = row.Cantidad,
                        PrecioUnitario = row.PrecioUnitario,
                        Subtotal = row.Subtotal
                    });
                }
            }

            return pedidoDict.Values.ToList();
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
            using var tx = _connection.BeginTransaction();

            pedido.Id = Guid.NewGuid();
            var rows = _connection.Execute(
                @"INSERT INTO Pedidos (Id, ClienteId, Estado, FechaCreacion, FechaConfirmacion, Total)
                  VALUES (@Id, @ClienteId, @Estado, @FechaCreacion, @FechaConfirmacion, @Total)
                  ON CONFLICT (Id) DO UPDATE SET
                      ClienteId = @ClienteId,
                      Estado = @Estado,
                      FechaCreacion = @FechaCreacion,
                      FechaConfirmacion = @FechaConfirmacion,
                      Total = @Total",
                new
                {
                    pedido.Id,
                    ClienteId = pedido.Cliente.Id,
                    Estado = (int)pedido.Estado,
                    pedido.FechaCreacion,
                    pedido.FechaConfirmacion,
                    pedido.Total
                },
                tx);

            _connection.Execute("DELETE FROM LineasPedido WHERE PedidoId = @PedidoId",
                new { PedidoId = pedido.Id }, tx);

            foreach (var linea in pedido.Detalles)
            {
                _connection.Execute(
                    @"INSERT INTO LineasPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario, Subtotal)
                      VALUES (@PedidoId, @ProductoId, @Cantidad, @PrecioUnitario, @Subtotal)",
                    new
                    {
                        PedidoId = pedido.Id,
                        ProductoId = linea.Producto.Id,
                        linea.Cantidad,
                        linea.PrecioUnitario,
                        linea.Subtotal
                    },
                    tx);
            }

            tx.Commit();
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar pedido {PedidoId}", pedido.Id);
            return false;
        }
    }
}
