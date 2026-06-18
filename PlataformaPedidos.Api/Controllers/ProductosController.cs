using Microsoft.AspNetCore.Mvc;
using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IServicioProducto _servicioProducto;
    private readonly IRepositorioProductos _repositorioProductos;

    public ProductosController(IServicioProducto servicioProducto, IRepositorioProductos repositorioProductos)
    {
        _servicioProducto = servicioProducto;
        _repositorioProductos = repositorioProductos;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repositorioProductos.GetAll());
    }

    [HttpPost]
    public IActionResult CrearProducto([FromBody] Producto producto)
    {
        Producto? resultado = _servicioProducto.CrearProducto(producto);
        if (resultado == null)
        {
            return BadRequest("No se pudo crear el producto. Verifique los datos ingresados.");
        }
        return Ok(resultado);
    }
}
