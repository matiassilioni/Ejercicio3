using PlataformaPedidos.Dominio.Contratos;

namespace PlataformaPedidos.Servicios;

public class ServicioNotificacion : IServicioNotificacion
{
    public bool Notificar()
    {
        var random = new Random();
        for (int i = 0; i < 100_000; i++)
        {
            double resultado = 0;
            for (int j = 0; j < 1000; j++)
            {
                resultado += Math.Sqrt(random.NextDouble() * j);
            }
        }

        return true;
    }
}
