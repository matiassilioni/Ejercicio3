using Azure.Storage.Queues;

const string ConnectionString =
    "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

const string QueueName = "pedidos";

var options = new QueueClientOptions(QueueClientOptions.ServiceVersion.V2025_11_05);
var queueClient = new QueueClient(ConnectionString, QueueName, options);

queueClient.CreateIfNotExists();

Console.WriteLine("Escuchando mensajes de la cola 'pedidos'. Presiona Ctrl+C para salir.");

while (true)
{
    var mensaje = queueClient.ReceiveMessage(TimeSpan.FromSeconds(10));

    if (mensaje.Value != null)
    {
        Console.WriteLine($"[{DateTime.UtcNow:O}] Mensaje recibido: {mensaje.Value.MessageText}");
        queueClient.DeleteMessage(mensaje.Value.MessageId, mensaje.Value.PopReceipt);
    }
    else
    {
        Thread.Sleep(1000);
    }
}
