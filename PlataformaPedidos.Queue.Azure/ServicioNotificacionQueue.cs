using Azure.Storage.Queues;
using PlataformaPedidos.Dominio.Contratos;

namespace PlataformaPedidos.Queue.Azure;

public class ServicioNotificacionQueue : IServicioNotificacion
{
    private readonly string _queueName;
    private readonly QueueClient _queueClient;

    public ServicioNotificacionQueue(string connectionString)
        : this(connectionString, "pedidos")
    {
    }

    public ServicioNotificacionQueue(string connectionString, string queueName)
    {
        _queueName = queueName;
        var options = new QueueClientOptions(QueueClientOptions.ServiceVersion.V2025_11_05);
        _queueClient = new QueueClient(connectionString, _queueName, options);
        _queueClient.CreateIfNotExists();
    }

    public bool Notificar(string mensaje)
    {
        try
        {
            _queueClient.SendMessage(mensaje);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
