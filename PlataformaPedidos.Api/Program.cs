using MySql.Data.MySqlClient;
using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Persistencia.MySQL;
using PlataformaPedidos.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("MySQL");

builder.Services.AddScoped<MySqlConnection>(_ =>
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
