using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.Mongodb;

public class RepositorioPedidos : IRepositorioPedidos
{
    private readonly IMongoCollection<Pedido> _pedidos;
    private readonly ILogger<RepositorioPedidos> _logger;

    public RepositorioPedidos(IMongoDatabase database, ILogger<RepositorioPedidos> logger)
    {
        _pedidos = database.GetCollection<Pedido>("pedidos");
        _logger = logger;
    }

    public Pedido GetById(Guid id)
    {
        try
        {
            return _pedidos.Find(p => p.Id == id).FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pedido con ID {PedidoId}", id);
            throw;
        }
    }

    public List<Pedido> GetAll()
    {
        try
        {
            return _pedidos.Find(_ => true).ToList();
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
            var result = _pedidos.ReplaceOne(
                p => p.Id == pedido.Id,
                pedido,
                new ReplaceOptions { IsUpsert = true });
            return result.IsAcknowledged;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar pedido con ID {PedidoId}", pedido.Id);
            throw;
        }
    }
}
