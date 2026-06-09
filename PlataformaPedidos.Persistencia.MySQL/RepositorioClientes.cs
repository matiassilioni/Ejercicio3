using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.MySQL;

public class RepositorioClientes : IRepositorioClientes
{
    private readonly string _connectionString;
    private readonly ILogger<RepositorioClientes> _logger;

    public RepositorioClientes(string connectionString, ILogger<RepositorioClientes> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public Cliente GetCliente(Guid id)
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            const string sql = "SELECT Id, Nombre FROM Clientes WHERE Id = @Id";
            return connection.QuerySingleOrDefault<Cliente>(sql, new { Id = id });
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
            using var connection = new MySqlConnection(_connectionString);
            const string sql = @"
                INSERT INTO Clientes (Id, Nombre)
                VALUES (@Id, @Nombre)
                ON DUPLICATE KEY UPDATE Nombre = @Nombre";
            var filas = connection.Execute(sql, new { cliente.Id, cliente.Nombre });
            return filas > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar cliente con Id {ClienteId}", cliente.Id);
            return false;
        }
    }
}
