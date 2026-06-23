using PlataformaPedidos.Servicios;

namespace PlataformaPedidos.Test;

public class ServicioNotificacionTest
{
    [Fact]
    public void Notificar_DebeRetornarTrue()
    {
        var servicio = new ServicioNotificacion();

        bool resultado = servicio.Notificar();

        Assert.True(resultado);
    }
}
