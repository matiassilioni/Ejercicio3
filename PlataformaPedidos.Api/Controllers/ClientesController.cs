using Microsoft.AspNetCore.Mvc;
using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IServicioCliente _servicioCliente;
    private readonly IRepositorioClientes _repositorioClientes;

    public ClientesController(IServicioCliente servicioCliente, IRepositorioClientes repositorioClientes)
    {
        _servicioCliente = servicioCliente;
        _repositorioClientes = repositorioClientes;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repositorioClientes.GetAll());
    }

    [HttpPost]
    public IActionResult CrearCliente([FromBody] string nombre)
    {
        Cliente? cliente = _servicioCliente.CrearCliente(nombre);
        if (cliente == null)
        {
            return BadRequest("No se pudo crear el cliente. Verifique que el nombre no esté vacío.");
        }
        return Ok(cliente);
    }
}
