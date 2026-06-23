using MySql.Data.MySqlClient;
using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Persistencia.MySQL;
using PlataformaPedidos.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("MySQL");

//AddScoped => crea la instancia y se la acuerda para cualquier servicio que pida este tipo
//otra vez durante lo que dure el request. 

//AddTransient => cada vez que se pida una instancia, se crea una nueva, no se acuerda de nada.

//Singleton => la primera vez que se pide la instancia, se crea y se guarda PARA SIEMPRE.
builder.Services.AddTransient<MySqlConnection>(_ =>
{
    var connection = new MySqlConnection(connectionString);
    connection.Open();
    return connection;
});

builder.Services.AddScoped<IRepositorioClientes>(sp =>
    new RepositorioClientes(sp.GetRequiredService<MySqlConnection>(), sp.GetRequiredService<ILogger<RepositorioClientes>>()));

builder.Services.AddScoped<IRepositorioProductos>(sp =>
    new RepositorioProductos(sp.GetRequiredService<MySqlConnection>(), sp.GetRequiredService<ILogger<RepositorioProductos>>()));

builder.Services.AddScoped<IRepositorioPedidos>(sp =>
    new RepositorioPedidos(sp.GetRequiredService<MySqlConnection>(), sp.GetRequiredService<ILogger<RepositorioPedidos>>()));

builder.Services.AddScoped<IServicioCliente, ServicioCliente>();
builder.Services.AddScoped<IServicioProducto, ServicioProducto>();
builder.Services.AddScoped<IServicioPedidos, ServicioPedidos>();
builder.Services.AddScoped<IServicioNotificacion, ServicioNotificacion>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
