using NSubstitute;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Enumeraciones;
using PlataformaPedidos.Dominio.Modelos;
using PlataformaPedidos.Servicios;

namespace PlataformaPedidos.Test;

public class ServicioPedidosTest
{
    [Fact]
    public void Test1()
    {
        IRepositorioPedidos repoMock = NSubstitute.Substitute.For<IRepositorioPedidos>();
        
        ServicioPedidos servicio = new ServicioPedidos(repoMock);
    }

    [Fact]
    public void CancelarPedidoPreviamenteCanceladoTest()
    {
        IRepositorioPedidos repoMock = NSubstitute.Substitute.For<IRepositorioPedidos>();
        ServicioPedidos servicio = new ServicioPedidos(repoMock);
        
        Guid pedidoId = Guid.NewGuid();

        Pedido pedido = new Pedido();
        pedido.Estado = EstadoPedido.Cancelado;
        
        repoMock.GetById(pedidoId).Returns(pedido);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        
        Assert.Equal(EstadoPedido.Cancelado, pedido.Estado);
        Assert.True(resultado);
        
        repoMock.Received(0).SaveOrUpdate(pedido);
        
    }

    [Fact]
    public void CancelarPedidoPendienteTest()
    {
        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        ServicioPedidos servicio = new ServicioPedidos(repoMock);
        
        Guid pedidoId = Guid.NewGuid();
        
        Pedido pedido = new Pedido();
        pedido.Estado = EstadoPedido.Pendiente;
        
        repoMock.GetById(pedidoId).Returns(pedido);
        repoMock.SaveOrUpdate(pedido).Returns(true);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        
        Assert.Equal(EstadoPedido.Cancelado, pedido.Estado);
        Assert.True(resultado);
    }
    
    
    [Fact]
    public void CancelarPedidoPendienteFallaGrabarTest()
    {
        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        ServicioPedidos servicio = new ServicioPedidos(repoMock);
        
        Guid pedidoId = Guid.NewGuid();
        
        Pedido pedido = new Pedido();
        pedido.Estado = EstadoPedido.Pendiente;
        
        repoMock.GetById(pedidoId).Returns(pedido);
        repoMock.SaveOrUpdate(pedido).Returns(false);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        
        Assert.Equal(EstadoPedido.Cancelado, pedido.Estado);
        Assert.False(resultado);
    }

    [Fact]
    public void CancelarPedidoConfirmadoTest()
    {
        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        ServicioPedidos servicio = new ServicioPedidos(repoMock);
        
        Guid pedidoId = Guid.NewGuid();
        
        Pedido pedido = new Pedido();
        pedido.Estado = EstadoPedido.Confirmado;
        
        repoMock.GetById(pedidoId).Returns(pedido);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        Assert.Equal(EstadoPedido.Confirmado, pedido.Estado);
        Assert. False(resultado);
    }

    [Fact]
    public void CancelarPedidoinexistenteTest()
    {
        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        ServicioPedidos servicio = new ServicioPedidos(repoMock);
        
        Guid pedidoId = Guid.NewGuid();
        
        repoMock.GetById(pedidoId).Returns((Pedido) null);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        Assert.False(resultado);
    }

}