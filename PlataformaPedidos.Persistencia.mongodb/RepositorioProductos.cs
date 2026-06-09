using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.Mongodb;

public class RepositorioProductos : IRepositorioProductos
{
    private readonly MongoDbContext _context;
    private readonly ILogger<RepositorioProductos> _logger;

    public RepositorioProductos(MongoDbContext context, ILogger<RepositorioProductos> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Producto GetById(Guid id)
    {
        try
        {
            return _context.Productos.Find(p => p.Id == id).FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto con ID {ProductoId}", id);
            return null;
        }
    }

    public List<Producto> GetAll()
    {
        try
        {
            return _context.Productos.Find(_ => true).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los productos");
            return new List<Producto>();
        }
    }

    public bool SaveOrUpdateProducto(Producto producto)
    {
        try
        {
            var result = _context.Productos.ReplaceOne(
                p => p.Id == producto.Id,
                producto,
                new ReplaceOptions { IsUpsert = true });
            return result.IsAcknowledged;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar producto con ID {ProductoId}", producto.Id);
            return false;
        }
    }
}
