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
    public void ConfirmarPedidoTestDePendienteAConfirmado()
    {
        // 1. ARRANGE (Preparar el escenario con NSubstitut
        var idDePrueba = Guid.NewGuid();
    
        // Creamos el pedido falso que va a devolver el repositorio simulado
        var pedidoFake = new Pedido
        {
            Id = idDePrueba,
            Estado = EstadoPedido.Pendiente,
            FechaConfirmacion = null
        };

        // Creamos el mock/substituto del repositorio
        IRepositorioPedidos repoMock = NSubstitute.Substitute.For<IRepositorioPedidos>();
    
        // CONFIGURACIÓN CLAVE: "Cuando llamen a GetById con el id de prueba, devolvé el pedidoFake"
        repoMock.GetById(idDePrueba).Returns(pedidoFake);
    
        // "Cuando llamen a SaveOrUpdate con cualquier Pedido, devolvé true"
        repoMock.SaveOrUpdate(Arg.Any<Pedido>()).Returns(true);

        // Inyectamos el mock en tu servicio real
        var servicio = new ServicioPedidos(repoMock);

        // 2. ACT (Ejecutar el método de tu servicio que queremos probar)
        bool resultado = servicio.ConfirmarPedido(idDePrueba);

        // 3. ASSERT (Verificaciones usando las aserciones de xUnit)
        Assert.True(resultado);
        Assert.Equal(EstadoPedido.Confirmado, pedidoFake.Estado);
        Assert.NotNull(pedidoFake.FechaConfirmacion);

       
        repoMock.Received(1).SaveOrUpdate(pedidoFake);
    }

    [Fact]
    public void ConfirmarPedidoTestIdInexistente()
    {
        var idDePrueba = Guid.NewGuid();
        // Creamos el mock/substituto del repositorio
        IRepositorioPedidos repoMock = NSubstitute.Substitute.For<IRepositorioPedidos>();
    
        // CONFIGURACIÓN CLAVE: "Cuando llamen a GetById con el id de prueba, devolvé el pedidoFake"
        repoMock.GetById(idDePrueba).Returns((Pedido)null);
        
        // Inyectamos el mock en tu servicio real
        var servicio = new ServicioPedidos(repoMock);

        // 2. ACT (Ejecutar el método de tu servicio que queremos probar)
        bool resultado = servicio.ConfirmarPedido(idDePrueba);
        
        Assert.False(resultado);
    }
    [Fact]
        public void ConfirmarPedido_CuandoElPedidoNoEstaPendiente_DebeRetornarFalseYNoGuardar()
        {
            // 1. ARRANGE
            var idDePrueba = Guid.NewGuid();
    
            // Creamos un pedido que ya fue procesado (por ejemplo, ya está Confirmado o Cancelado)
            var pedidoFake = new Pedido
            {
                Id = idDePrueba,
                Estado = EstadoPedido.Confirmado, // No está Pendiente
                FechaConfirmacion = DateTime.UtcNow.AddDays(-1)
            };
    
            IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
            repoMock.GetById(idDePrueba).Returns(pedidoFake);
            
            var servicio = new ServicioPedidos(repoMock);
    
            // 2. ACT
            bool resultado = servicio.ConfirmarPedido(idDePrueba);
    
            // 3. ASSERT
            Assert.False(resultado);
            // Verificación clave: Como el estado no era Pendiente, el método debió cortar antes y NO guardar
            repoMock.DidNotReceive().SaveOrUpdate(Arg.Any<Pedido>());
        }
    
    [Fact]    
    public void ConfirmarPedido_CuandoElRepositorioFallaAlGuardar_DebeRetornarFalse()
    {
        // 1. ARRANGE
        var idDePrueba = Guid.NewGuid();
        var pedidoFake = new Pedido { Id = idDePrueba, Estado = EstadoPedido.Pendiente };

        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        repoMock.GetById(idDePrueba).Returns(pedidoFake);
    
        // CONFIGURACIÓN CLAVE: El pedido es válido, pero el guardado falla (devuelve false)
        repoMock.SaveOrUpdate(Arg.Any<Pedido>()).Returns(false);

        var servicio = new ServicioPedidos(repoMock);

        // 2. ACT
        bool resultado = servicio.ConfirmarPedido(idDePrueba);

        // 3. ASSERT
        Assert.False(resultado);
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