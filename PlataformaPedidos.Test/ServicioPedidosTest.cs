using PlataformaPedidos.Dominio.Contratos.Persistencia;
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
    
}