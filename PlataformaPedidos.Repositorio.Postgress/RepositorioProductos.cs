using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Repositorio.Postgress;

public class RepositorioProductos : IRepositorioProductos
{
    private readonly string _connectionString;
    private readonly ILogger<RepositorioProductos> _logger;

    public RepositorioProductos(string connectionString, ILogger<RepositorioProductos> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public Producto GetById(Guid id)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return connection.QueryFirstOrDefault<Producto>(
                "SELECT Id, Nombre, Descripcion, Precio, FechaModificacion, Disponible FROM Productos WHERE Id = @Id",
                new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto con Id {ProductoId}", id);
            throw;
        }
    }

    public List<Producto> GetAll()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return connection.Query<Producto>(
                "SELECT Id, Nombre, Descripcion, Precio, FechaModificacion, Disponible FROM Productos").ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los productos");
            throw;
        }
    }

    public bool SaveOrUpdateProducto(Producto producto)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var filas = connection.Execute(
                @"INSERT INTO Productos (Id, Nombre, Descripcion, Precio, FechaModificacion, Disponible)
                  VALUES (@Id, @Nombre, @Descripcion, @Precio, @FechaModificacion, @Disponible)
                  ON CONFLICT (Id) DO UPDATE SET
                      Nombre = @Nombre,
                      Descripcion = @Descripcion,
                      Precio = @Precio,
                      FechaModificacion = @FechaModificacion,
                      Disponible = @Disponible",
                producto);
            return filas > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar producto con Id {ProductoId}", producto.Id);
            throw;
        }
    }
}
