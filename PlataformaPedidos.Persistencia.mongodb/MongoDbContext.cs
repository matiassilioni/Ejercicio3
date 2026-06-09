using MongoDB.Driver;

namespace PlataformaPedidos.Persistencia.Mongodb;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Dominio.Modelos.Cliente> Clientes =>
        _database.GetCollection<Dominio.Modelos.Cliente>("clientes");

    public IMongoCollection<Dominio.Modelos.Pedido> Pedidos =>
        _database.GetCollection<Dominio.Modelos.Pedido>("pedidos");

    public IMongoCollection<Dominio.Modelos.Producto> Productos =>
        _database.GetCollection<Dominio.Modelos.Producto>("productos");
}
