using NSubstitute;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;
using PlataformaPedidos.Servicios;

namespace PlataformaPedidos.Test;

public class CancelarPedidoTest
{
    [Fact]
    public void CancelarPedidoPendienteTest()
    {
        Guid pedidoId = Guid.NewGuid();
        Pedido pedido = new Pedido();
        IRepositorioPedidos repoMock = NSubstitute.Substitute.For<IRepositorioPedidos>();
        repoMock.GetById(pedidoId).Returns(pedido);
        repoMock.SaveOrUpdate(pedido).Returns(true);

        ServicioPedidos servicio = new ServicioPedidos(repoMock);

        Boolean res = servicio.CancelarPedido(pedidoId);

        Assert.True(res);
    }

    [Fact]
    public void CancelarPedidoNoExisteTest()
    {
        Guid pedidoId = Guid.NewGuid();
        IRepositorioPedidos repoMock = NSubstitute.Substitute.For<IRepositorioPedidos>();
        repoMock.GetById(pedidoId).Returns((Pedido)null);

        ServicioPedidos servicio = new ServicioPedidos(repoMock);

        Boolean res = servicio.CancelarPedido(pedidoId);

        Assert.False(res);
    }
    
}