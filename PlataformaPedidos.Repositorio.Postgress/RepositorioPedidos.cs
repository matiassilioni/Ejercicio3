using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Repositorio.Postgress;

public class RepositorioPedidos : IRepositorioPedidos
{
    private readonly string _connectionString;
    private readonly ILogger<RepositorioPedidos> _logger;

    public RepositorioPedidos(string connectionString, ILogger<RepositorioPedidos> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public Pedido GetById(Guid id)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var pedido = connection.QueryFirstOrDefault<Pedido>(
                "SELECT Id, Estado, FechaCreacion, FechaConfirmacion, Total FROM Pedidos WHERE Id = @Id",
                new { Id = id });

            if (pedido == null)
                return null;

            pedido.Cliente = connection.QueryFirstOrDefault<Cliente>(
                @"SELECT c.Id, c.Nombre FROM Clientes c
                  INNER JOIN Pedidos p ON p.ClienteId = c.Id
                  WHERE p.Id = @Id",
                new { Id = id });

            pedido.Detalles = connection.Query<LineaPedido>(
                @"SELECT lp.Cantidad, lp.PrecioUnitario, lp.Subtotal,
                         pr.Id, pr.Nombre, pr.Descripcion, pr.Precio, pr.FechaModificacion, pr.Disponible
                  FROM LineasPedido lp
                  INNER JOIN Productos pr ON pr.Id = lp.ProductoId
                  WHERE lp.PedidoId = @Id",
                new { Id = id }).AsList();

            return pedido;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pedido con Id {PedidoId}", id);
            throw;
        }
    }

    public List<Pedido> GetAll()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return connection.Query<Pedido>(
                "SELECT Id, Estado, FechaCreacion, FechaConfirmacion, Total FROM Pedidos").ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los pedidos");
            throw;
        }
    }

    public bool SaveOrUpdate(Pedido pedido)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var filas = connection.Execute(
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
                    ClienteId = pedido.Cliente?.Id,
                    pedido.Estado,
                    pedido.FechaCreacion,
                    pedido.FechaConfirmacion,
                    pedido.Total
                }, transaction);

            if (filas <= 0)
            {
                transaction.Rollback();
                return false;
            }

            connection.Execute("DELETE FROM LineasPedido WHERE PedidoId = @PedidoId",
                new { PedidoId = pedido.Id }, transaction);

            foreach (var linea in pedido.Detalles)
            {
                connection.Execute(
                    @"INSERT INTO LineasPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario, Subtotal)
                      VALUES (@PedidoId, @ProductoId, @Cantidad, @PrecioUnitario, @Subtotal)",
                    new
                    {
                        PedidoId = pedido.Id,
                        ProductoId = linea.Producto.Id,
                        linea.Cantidad,
                        linea.PrecioUnitario,
                        linea.Subtotal
                    }, transaction);
            }

            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar pedido con Id {PedidoId}", pedido.Id);
            throw;
        }
    }
}
