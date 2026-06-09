using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.Postgress;

public class RepositorioProductos : IRepositorioProductos
{
    private readonly IDbConnection _connection;
    private readonly ILogger<RepositorioProductos> _logger;

    public RepositorioProductos(IDbConnection connection, ILogger<RepositorioProductos> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public Producto GetById(Guid id)
    {
        try
        {
            return _connection.QueryFirstOrDefault<Producto>(
                "SELECT Id, Nombre, Descripcion, Precio, FechaModificacion, Disponible FROM Productos WHERE Id = @Id",
                new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto con Id {ProductoId}", id);
            return null;
        }
    }

    public List<Producto> GetAll()
    {
        try
        {
            return _connection.Query<Producto>(
                "SELECT Id, Nombre, Descripcion, Precio, FechaModificacion, Disponible FROM Productos").AsList();
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
            producto.Id = Guid.NewGuid();
            var rows = _connection.Execute(
                @"INSERT INTO Productos (Id, Nombre, Descripcion, Precio, FechaModificacion, Disponible)
                  VALUES (@Id, @Nombre, @Descripcion, @Precio, @FechaModificacion, @Disponible)
                  ON CONFLICT (Id) DO UPDATE SET
                      Nombre = @Nombre,
                      Descripcion = @Descripcion,
                      Precio = @Precio,
                      FechaModificacion = @FechaModificacion,
                      Disponible = @Disponible",
                producto);
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar producto {ProductoId}", producto.Id);
            return false;
        }
    }
}
