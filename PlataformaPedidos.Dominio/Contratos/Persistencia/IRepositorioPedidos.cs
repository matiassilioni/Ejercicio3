using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Dominio.Contratos.Persistencia;

public interface IRepositorioPedidos
{
    public Pedido GetById(Guid id);
    public List<Pedido> GetAll();
    public bool SaveOrUpdate(Pedido pedido);
}