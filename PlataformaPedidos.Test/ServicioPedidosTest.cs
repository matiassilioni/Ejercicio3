using NSubstitute;
using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Enumeraciones;
using PlataformaPedidos.Dominio.Modelos;
using PlataformaPedidos.Servicios;

namespace PlataformaPedidos.Test;

public class ServicioPedidosTest
{
    private static (IRepositorioPedidos, IRepositorioClientes, IRepositorioProductos, IServicioNotificacion) CrearMocks()
    {
        return (
            Substitute.For<IRepositorioPedidos>(),
            Substitute.For<IRepositorioClientes>(),
            Substitute.For<IRepositorioProductos>(),
            Substitute.For<IServicioNotificacion>()
        );
    }

    [Fact]
    public void Test1()
    {
        var (repoMock, _, _, _) = CrearMocks();
        
        var (_, clientesMock, productosMock, _) = CrearMocks();
        ServicioPedidos servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());
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
        var (_, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

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
        IRepositorioPedidos repoMock = NSubstitute.Substitute.For<IRepositorioPedidos>();
        repoMock.GetById(idDePrueba).Returns((Pedido)null);
        
        var (_, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        bool resultado = servicio.ConfirmarPedido(idDePrueba);
        
        Assert.False(resultado);
    }
    
    [Fact]
    public void ConfirmarPedido_CuandoElPedidoNoEstaPendiente_DebeRetornarFalseYNoGuardar()
    {
        var idDePrueba = Guid.NewGuid();
        var pedidoFake = new Pedido
        {
            Id = idDePrueba,
            Estado = EstadoPedido.Confirmado,
            FechaConfirmacion = DateTime.UtcNow.AddDays(-1)
        };

        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        repoMock.GetById(idDePrueba).Returns(pedidoFake);
        
        var (_, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        bool resultado = servicio.ConfirmarPedido(idDePrueba);

        Assert.False(resultado);
        repoMock.DidNotReceive().SaveOrUpdate(Arg.Any<Pedido>());
    }
    
    [Fact]    
    public void ConfirmarPedido_CuandoElRepositorioFallaAlGuardar_DebeRetornarFalse()
    {
        var idDePrueba = Guid.NewGuid();
        var pedidoFake = new Pedido { Id = idDePrueba, Estado = EstadoPedido.Pendiente };

        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        repoMock.GetById(idDePrueba).Returns(pedidoFake);
        repoMock.SaveOrUpdate(Arg.Any<Pedido>()).Returns(false);

        var (_, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        bool resultado = servicio.ConfirmarPedido(idDePrueba);

        Assert.False(resultado);
    }

    [Fact]
    public void CancelarPedidoPreviamenteCanceladoTest()
    {
        IRepositorioPedidos repoMock = NSubstitute.Substitute.For<IRepositorioPedidos>();
        var (_, clientesMock, productosMock, _) = CrearMocks();
        ServicioPedidos servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());
        
        Guid pedidoId = Guid.NewGuid();

        Pedido pedido = new Pedido();
        pedido.Estado = EstadoPedido.Cancelado;
        
        repoMock.GetById(pedidoId).Returns(pedido);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        
        Assert.Equal(EstadoPedido.Cancelado, pedido.Estado);
        Assert.True(resultado);
        
        repoMock.Received(1).GetById(pedidoId);
        repoMock.Received(0).SaveOrUpdate(pedido);
    }

    [Fact]
    public void CancelarPedidoPendienteTest()
    {
        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        var (_, clientesMock, productosMock, _) = CrearMocks();
        ServicioPedidos servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());
        
        Guid pedidoId = Guid.NewGuid();
        
        Pedido pedido = new Pedido();
        pedido.Estado = EstadoPedido.Pendiente;
        
        repoMock.GetById(pedidoId).Returns(pedido);
        repoMock.SaveOrUpdate(pedido).Returns(true);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        
        Assert.Equal(EstadoPedido.Cancelado, pedido.Estado);
        Assert.True(resultado);
        
        repoMock.Received(1).GetById(pedidoId);
        repoMock.Received(1).SaveOrUpdate(pedido);
    }
    
    [Fact]
    public void CancelarPedidoPendienteFallaGrabarTest()
    {
        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        var (_, clientesMock, productosMock, _) = CrearMocks();
        ServicioPedidos servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());
        
        Guid pedidoId = Guid.NewGuid();
        
        Pedido pedido = new Pedido();
        pedido.Estado = EstadoPedido.Pendiente;
        
        repoMock.GetById(pedidoId).Returns(pedido);
        repoMock.SaveOrUpdate(pedido).Returns(false);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        
        Assert.Equal(EstadoPedido.Cancelado, pedido.Estado);
        Assert.False(resultado);
        
        repoMock.Received(1).GetById(pedidoId);
        repoMock.Received(1).SaveOrUpdate(pedido);
    }

    [Fact]
    public void CancelarPedidoConfirmadoTest()
    {
        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        var (_, clientesMock, productosMock, _) = CrearMocks();
        ServicioPedidos servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());
        
        Guid pedidoId = Guid.NewGuid();
        
        Pedido pedido = new Pedido();
        pedido.Estado = EstadoPedido.Confirmado;
        
        repoMock.GetById(pedidoId).Returns(pedido);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        Assert.Equal(EstadoPedido.Confirmado, pedido.Estado);
        Assert.False(resultado);
        
        repoMock.Received(1).GetById(pedidoId);
        repoMock.Received(0).SaveOrUpdate(pedido);
    }

    [Fact]
    public void CancelarPedidoinexistenteTest()
    {
        IRepositorioPedidos repoMock = Substitute.For<IRepositorioPedidos>();
        var (_, clientesMock, productosMock, _) = CrearMocks();
        ServicioPedidos servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());
        
        Guid pedidoId = Guid.NewGuid();
        
        repoMock.GetById(pedidoId).Returns((Pedido) null);
        
        bool resultado = servicio.CancelarPedido(pedidoId);
        Assert.False(resultado);
        
        repoMock.Received(1).GetById(pedidoId);
        repoMock.DidNotReceive().SaveOrUpdate(Arg.Any<Pedido>());
    }

    [Fact]
    public void CrearPedido_CuandoTodoEsValido_DebeRetornarId()
    {
        var (repoMock, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        var clienteExistente = new Cliente { Id = Guid.NewGuid(), Nombre = "Test" };
        var productoExistente = new Producto { Id = Guid.NewGuid(), Nombre = "Prod1", Precio = 100m };
        var pedido = new Pedido
        {
            Cliente = new Cliente { Id = clienteExistente.Id },
            Detalles = new List<LineaPedido>
            {
                new LineaPedido { Producto = new Producto { Id = productoExistente.Id }, Cantidad = 2 }
            }
        };

        clientesMock.GetCliente(clienteExistente.Id).Returns(clienteExistente);
        productosMock.GetById(productoExistente.Id).Returns(productoExistente);
        repoMock.SaveOrUpdate(Arg.Any<Pedido>()).Returns(true);

        Guid? resultado = servicio.CrearPedido(pedido);

        Assert.NotNull(resultado);
        Assert.NotEqual(Guid.Empty, resultado.Value);
        Assert.Equal(EstadoPedido.Pendiente, pedido.Estado);
        Assert.Equal(200m, pedido.Total);
        repoMock.Received(1).SaveOrUpdate(Arg.Any<Pedido>());
    }

    [Fact]
    public void CrearPedido_CuandoPedidoEsNull_DebeRetornarNull()
    {
        var (repoMock, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        Guid? resultado = servicio.CrearPedido(null);

        Assert.Null(resultado);
        repoMock.DidNotReceive().SaveOrUpdate(Arg.Any<Pedido>());
    }

    [Fact]
    public void CrearPedido_CuandoClienteEsNull_DebeRetornarNull()
    {
        var (repoMock, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        var pedido = new Pedido { Cliente = null };
        Guid? resultado = servicio.CrearPedido(pedido);

        Assert.Null(resultado);
        repoMock.DidNotReceive().SaveOrUpdate(Arg.Any<Pedido>());
    }

    [Fact]
    public void CrearPedido_CuandoClienteNoExiste_DebeRetornarNull()
    {
        var (repoMock, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        var pedido = new Pedido { Cliente = new Cliente { Id = Guid.NewGuid() } };
        clientesMock.GetCliente(Arg.Any<Guid>()).Returns((Cliente)null);

        Guid? resultado = servicio.CrearPedido(pedido);

        Assert.Null(resultado);
        repoMock.DidNotReceive().SaveOrUpdate(Arg.Any<Pedido>());
    }

    [Fact]
    public void CrearPedido_CuandoNoHayDetalles_DebeRetornarNull()
    {
        var (repoMock, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        var clienteExistente = new Cliente { Id = Guid.NewGuid(), Nombre = "Test" };
        var pedido = new Pedido
        {
            Cliente = new Cliente { Id = clienteExistente.Id },
            Detalles = null
        };

        clientesMock.GetCliente(clienteExistente.Id).Returns(clienteExistente);

        Guid? resultado = servicio.CrearPedido(pedido);

        Assert.Null(resultado);
        repoMock.DidNotReceive().SaveOrUpdate(Arg.Any<Pedido>());
    }

    [Fact]
    public void CrearPedido_CuandoProductoNoExiste_DebeRetornarNull()
    {
        var (repoMock, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        var clienteExistente = new Cliente { Id = Guid.NewGuid(), Nombre = "Test" };
        var pedido = new Pedido
        {
            Cliente = new Cliente { Id = clienteExistente.Id },
            Detalles = new List<LineaPedido>
            {
                new LineaPedido { Producto = new Producto { Id = Guid.NewGuid() }, Cantidad = 1 }
            }
        };

        clientesMock.GetCliente(clienteExistente.Id).Returns(clienteExistente);
        productosMock.GetById(Arg.Any<Guid>()).Returns((Producto)null);

        Guid? resultado = servicio.CrearPedido(pedido);

        Assert.Null(resultado);
        repoMock.DidNotReceive().SaveOrUpdate(Arg.Any<Pedido>());
    }

    [Fact]
    public void CrearPedido_CuandoRepositorioFalla_DebeRetornarNull()
    {
        var (repoMock, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        var clienteExistente = new Cliente { Id = Guid.NewGuid(), Nombre = "Test" };
        var productoExistente = new Producto { Id = Guid.NewGuid(), Nombre = "Prod1", Precio = 50m };
        var pedido = new Pedido
        {
            Cliente = new Cliente { Id = clienteExistente.Id },
            Detalles = new List<LineaPedido>
            {
                new LineaPedido { Producto = new Producto { Id = productoExistente.Id }, Cantidad = 3 }
            }
        };

        clientesMock.GetCliente(clienteExistente.Id).Returns(clienteExistente);
        productosMock.GetById(productoExistente.Id).Returns(productoExistente);
        repoMock.SaveOrUpdate(Arg.Any<Pedido>()).Returns(false);

        Guid? resultado = servicio.CrearPedido(pedido);

        Assert.Null(resultado);
        repoMock.Received(1).SaveOrUpdate(Arg.Any<Pedido>());
    }

    [Fact]
    public void CrearPedido_CuandoDetallesVacio_DebeRetornarNull()
    {
        var (repoMock, clientesMock, productosMock, _) = CrearMocks();
        var servicio = new ServicioPedidos(repoMock, clientesMock, productosMock, Substitute.For<IServicioNotificacion>());

        var clienteExistente = new Cliente { Id = Guid.NewGuid(), Nombre = "Test" };
        var pedido = new Pedido
        {
            Cliente = new Cliente { Id = clienteExistente.Id },
            Detalles = new List<LineaPedido>()
        };

        clientesMock.GetCliente(clienteExistente.Id).Returns(clienteExistente);

        Guid? resultado = servicio.CrearPedido(pedido);

        Assert.Null(resultado);
        repoMock.DidNotReceive().SaveOrUpdate(Arg.Any<Pedido>());
    }
}   