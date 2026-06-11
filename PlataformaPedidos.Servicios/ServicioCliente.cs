using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Servicios;

public class ServicioCliente : IServicioCliente
{
    private IRepositorioClientes _repositorioClientes;

    public ServicioCliente(IRepositorioClientes repositorioClientes)
    {
        _repositorioClientes = repositorioClientes;
    }

    public Cliente CrearCliente(string nombre)
    {
        if (string.IsNullOrEmpty(nombre))
        {
            return null;
        }

        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nombre = nombre
        };

        bool graboBien = _repositorioClientes.SaveOrUpdateCliente(cliente);

        if (!graboBien)
        {
            return null;
        }

        return cliente;
    }
}
