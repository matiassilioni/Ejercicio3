using System.IO.MemoryMappedFiles;
using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Enumeraciones;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Servicios;

public class ServicioPedidos : IServicioPedidos
{
    private readonly IRepositorioPedidos _repositorioPedidos;
    private readonly IRepositorioClientes _repositorioClientes;
    private readonly IRepositorioProductos _repositorioProductos;

    public ServicioPedidos(IRepositorioPedidos repositorioPedidos, IRepositorioClientes  repositorioClientes
        , IRepositorioProductos repositorioProductos)
    {
        _repositorioPedidos = repositorioPedidos;
        _repositorioClientes = repositorioClientes;
        _repositorioProductos = repositorioProductos;
    }
    
    public Guid? CrearPedido(Pedido pedido)
    {
        if (pedido == null) return null;

        if (pedido.Cliente == null) return null;

        Cliente clienteDeLaBase = _repositorioClientes.GetCliente(pedido.Cliente.Id);

        if (clienteDeLaBase == null) return null;
        
        pedido.Cliente = clienteDeLaBase;
        
        if (pedido.Detalles == null || pedido.Detalles.Count == 0) return null;

        foreach (LineaPedido pedidoDetalle in pedido.Detalles)
        {
            Producto productoDeLaBase = _repositorioProductos.GetById(pedidoDetalle.Producto.Id);
            if (productoDeLaBase == null) return null;
            
            pedidoDetalle.Producto = productoDeLaBase;
            pedidoDetalle.PrecioUnitario = productoDeLaBase.Precio;
            pedidoDetalle.Subtotal = pedidoDetalle.PrecioUnitario * pedidoDetalle.Cantidad;
        }
        
        pedido.Id = Guid.NewGuid();
        pedido.Estado = EstadoPedido.Pendiente;
        pedido.FechaConfirmacion = null;
        pedido.FechaCreacion = DateTime.UtcNow;
        
        pedido.Total = pedido.Detalles.Sum(lineaPedido => lineaPedido.Subtotal);
        
        bool graboBien = _repositorioPedidos.SaveOrUpdate(pedido);
        
        return graboBien ? pedido.Id : null;
    }

    public bool ConfirmarPedido(Guid pedidoId)
    {
        Pedido pedido = _repositorioPedidos.GetById(pedidoId);

        if (pedido == null)
        {
            return false;
        }

        if (pedido.Estado != EstadoPedido.Pendiente)
        {
            return false;
        }

        pedido.Estado = EstadoPedido.Confirmado;
        pedido.FechaConfirmacion = DateTime.UtcNow;

        bool graboBien = _repositorioPedidos.SaveOrUpdate(pedido);
        
        return graboBien;
        //Boca Jr 
  }
     public bool CancelarPedido(Guid pedidoId)
    {
        
        Pedido pedido = _repositorioPedidos.GetById(pedidoId);
        if (pedido == null)
        {
            return false;
        }

        if (pedido.Estado == EstadoPedido.Cancelado)
        {
            return true;
        }

        if (pedido.Estado == EstadoPedido.Pendiente)
        {
            pedido.Estado = EstadoPedido.Cancelado;
            return _repositorioPedidos.SaveOrUpdate(pedido);
        }

        return false;
    }
}