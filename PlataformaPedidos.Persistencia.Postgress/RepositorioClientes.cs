using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Persistencia.Postgress;

public class RepositorioClientes : IRepositorioClientes
{
    private readonly IDbConnection _connection;
    private readonly ILogger<RepositorioClientes> _logger;

    public RepositorioClientes(IDbConnection connection, ILogger<RepositorioClientes> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public Cliente GetCliente(Guid id)
    {
        try
        {
            return _connection.QueryFirstOrDefault<Cliente>(
                "SELECT Id, Nombre FROM Clientes WHERE Id = @Id",
                new { Id = id });
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
            cliente.Id = Guid.NewGuid();
            var rows = _connection.Execute(
                @"INSERT INTO Clientes (Id, Nombre)
                  VALUES (@Id, @Nombre)
                  ON CONFLICT (Id) DO UPDATE SET Nombre = @Nombre",
                cliente);
            return rows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar o actualizar cliente {ClienteId}", cliente.Id);
            return false;
        }
    }
}
