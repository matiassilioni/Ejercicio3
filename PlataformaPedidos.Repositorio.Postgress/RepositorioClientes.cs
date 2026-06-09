using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Repositorio.Postgress;

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
            using var connection = new NpgsqlConnection(_connectionString);
            return connection.QueryFirstOrDefault<Cliente>(
                "SELECT Id, Nombre FROM Clientes WHERE Id = @Id",
                new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente con Id {ClienteId}", id);
            throw;
        }
    }

    public bool SaveOrUpdateCliente(Cliente cliente)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var filas = connection.Execute(
                @"INSERT INTO Clientes (Id, Nombre)
                  VALUES (@Id, @Nombre)
                  ON CONFLICT (Id) DO UPDATE SET
                      Nombre = @Nombre",
                cliente);
            return filas > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar cliente con Id {ClienteId}", cliente.Id);
            throw;
        }
    }
}
