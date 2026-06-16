using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.MySQL;

public class RepositorioClientes : IRepositorioClientes
{
    private readonly MySqlConnection _connection;
    private readonly ILogger<RepositorioClientes> _logger;

    public RepositorioClientes(MySqlConnection connection, ILogger<RepositorioClientes> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public List<Cliente> GetAll()
    {
        try
        {
            const string sql = "SELECT Id, Nombre FROM Clientes";
            return _connection.Query<Cliente>(sql).AsList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los clientes");
            return new List<Cliente>();
        }
    }

    public Cliente GetCliente(Guid id)
    {
        try
        {
            const string sql = "SELECT Id, Nombre FROM Clientes WHERE Id = @Id";
            return _connection.QuerySingleOrDefault<Cliente>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente con Id {ClienteId}", id);
            return null;
        }
    }

    public bool SaveOrUpdateCliente(Cliente cliente)
    {
        try
        {
            const string sql = @"
                INSERT INTO Clientes (Id, Nombre)
                VALUES (@Id, @Nombre)
                ON DUPLICATE KEY UPDATE Nombre = @Nombre";
            var filas = _connection.Execute(sql, new { cliente.Id, cliente.Nombre });
            return filas > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar cliente con Id {ClienteId}", cliente.Id);
            return false;
        }
    }
}
