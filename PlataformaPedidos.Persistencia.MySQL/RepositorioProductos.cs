using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.MySQL;

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
            using var connection = new MySqlConnection(_connectionString);
            const string sql = @"
                SELECT Id, Nombre, Descripcion, Precio, FechaModificacion, Disponible
                FROM Productos WHERE Id = @Id";
            return connection.QuerySingleOrDefault<Producto>(sql, new { Id = id });
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
            using var connection = new MySqlConnection(_connectionString);
            const string sql = @"
                SELECT Id, Nombre, Descripcion, Precio, FechaModificacion, Disponible
                FROM Productos";
            return connection.Query<Producto>(sql).AsList();
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
            using var connection = new MySqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO Productos (Id, Nombre, Descripcion, Precio, FechaModificacion, Disponible)
                VALUES (@Id, @Nombre, @Descripcion, @Precio, @FechaModificacion, @Disponible)
                ON DUPLICATE KEY UPDATE
                    Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Precio = @Precio,
                    FechaModificacion = @FechaModificacion,
                    Disponible = @Disponible";
            var filas = connection.Execute(sql, new
            {
                producto.Id,
                producto.Nombre,
                producto.Descripcion,
                producto.Precio,
                producto.FechaModificacion,
                producto.Disponible
            });
            return filas > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar producto con Id {ProductoId}", producto.Id);
            return false;
        }
    }
}
