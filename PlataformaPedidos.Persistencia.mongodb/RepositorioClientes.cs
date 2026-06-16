using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.Mongodb;

public class RepositorioClientes : IRepositorioClientes
{
    private readonly MongoDbContext _context;
    private readonly ILogger<RepositorioClientes> _logger;

    public RepositorioClientes(MongoDbContext context, ILogger<RepositorioClientes> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Cliente> GetAll()
    {
        try
        {
            return _context.Clientes.Find(_ => true).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los clientes");
            throw;
        }
    }

    public Cliente GetCliente(Guid id)
    {
        try
        {
            return _context.Clientes.Find(c => c.Id == id).FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente con ID {ClienteId}", id);
            throw;
        }
    }

    public bool SaveOrUpdateCliente(Cliente cliente)
    {
        try
        {
            var result = _context.Clientes.ReplaceOne(
                c => c.Id == cliente.Id,
                cliente,
                new ReplaceOptions { IsUpsert = true });
            return result.IsAcknowledged;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar cliente con ID {ClienteId}", cliente.Id);
            throw;
        }
    }
}
