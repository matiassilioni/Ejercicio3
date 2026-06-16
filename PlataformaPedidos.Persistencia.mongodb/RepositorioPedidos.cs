using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.Mongodb;

public class RepositorioPedidos : IRepositorioPedidos
{
    private readonly MongoDbContext _context;
    private readonly ILogger<RepositorioPedidos> _logger;

    public RepositorioPedidos(MongoDbContext context, ILogger<RepositorioPedidos> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Pedido GetById(Guid id)
    {
        try
        {
            return _context.Pedidos.Find(p => p.Id == id).FirstOrDefault();
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
            return _context.Pedidos.Find(_ => true).ToList();
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
            var result = _context.Pedidos.ReplaceOne(
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
