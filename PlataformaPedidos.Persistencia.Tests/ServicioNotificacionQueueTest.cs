using Azure.Storage.Queues;
using PlataformaPedidos.Queue.Azure;

namespace PlataformaPedidos.Persistencia.Tests;

public class ServicioNotificacionQueueTest
{
    private const string ConnectionString =
        "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

    private const string QueueName = "pedidos-test";

    private static readonly QueueClientOptions Options = new(QueueClientOptions.ServiceVersion.V2025_11_05);

    public ServicioNotificacionQueueTest()
    {
        var queueClient = new QueueClient(ConnectionString, QueueName, Options);
        queueClient.DeleteIfExists();
    }

    [Fact]
    public void Notificar_EnvíaMensajeALaCola_Y_PuedeLeerse()
    {
        var mensajeEsperado = "test-mensaje:Confirmado";

        var servicio = new ServicioNotificacionQueue(ConnectionString, QueueName);
        var resultado = servicio.Notificar(mensajeEsperado);

        Assert.True(resultado);

        var queueClient = new QueueClient(ConnectionString, QueueName, Options);
        var mensajeRecibido = queueClient.ReceiveMessage(TimeSpan.FromSeconds(5));

        Assert.NotNull(mensajeRecibido.Value);
        Assert.Equal(mensajeEsperado, mensajeRecibido.Value.MessageText);
    }
}
