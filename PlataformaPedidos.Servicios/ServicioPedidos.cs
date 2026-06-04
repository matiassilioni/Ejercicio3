using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Enumeraciones;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Servicios;

public class ServicioPedidos : IServicioPedidos
{
    private IRepositorioPedidos _repositorioPedidos;
    
    public ServicioPedidos(IRepositorioPedidos repositorioPedidos)
    {
        _repositorioPedidos = repositorioPedidos;
    }
    
    public bool CrearPedido(Pedido pedido)
    {
        if (pedido == null)
        {
            return false;
        }

        if (pedido.Cliente == null)
        {
            return false;
        }

        if (pedido.Detalles == null || pedido.Detalles.Count == 0)
        {
            return false;
        }

        pedido.Estado = EstadoPedido.Pendiente;
        pedido.FechaConfirmacion = null;
        pedido.FechaCreacion = DateTime.UtcNow;
        
        decimal total = 0;
        foreach (LineaPedido linea in pedido.Detalles)
        {
            total = total + linea.Cantidad * linea.PrecioUnitario;
        } 
        
        pedido.Total = total;
        
        //linq
        pedido.Total = pedido.Detalles.Sum(lineaPedido => lineaPedido.Cantidad * lineaPedido.PrecioUnitario);
        
        
        bool graboBien = _repositorioPedidos.SaveOrUpdate(pedido);
        
        return graboBien;
    }

    public bool ConfirmarPedido(Guid pedidoId)
    {
        throw new NotImplementedException();
    }

    public bool CancelarPedido(Guid pedidoId)
    {
        
        Pedido pedido = _repositorioPedidos.GetById(pedidoId);

        if (pedido == null || pedidoId == Guid.Empty)
        {
            return false;
        }

        if (pedido.Estado == EstadoPedido.Cancelado)
        {
            return true;
        }
        
        if (pedido.Estado != EstadoPedido.Cancelado)
        {
            pedido.Estado = EstadoPedido.Cancelado;
            return _repositorioPedidos.SaveOrUpdate(pedido);
        }
        return false;
    }
}