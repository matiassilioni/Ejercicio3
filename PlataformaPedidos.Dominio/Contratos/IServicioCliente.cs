using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Dominio.Contratos;

public interface IServicioCliente
{
    public Cliente CrearCliente(string nombre);
}