using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Dominio.Contratos.Persistencia;

public interface IRepositorioClientes
{
    public Cliente GetCliente(Guid id);
    public bool SaveOrUpdateCliente(Cliente cliente);
}