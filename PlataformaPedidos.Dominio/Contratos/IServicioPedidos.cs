using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Dominio.Contratos;

public interface IServicioPedidos
{
    public bool CrearPedido(Pedido pedido);
    
    public bool ConfirmarPedido(Guid pedidoId);
    
    public bool CancelarPedido(Guid pedidoId);
    
}