using Microsoft.AspNetCore.Mvc;
using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IServicioPedidos _servicioPedidos;
    private readonly IRepositorioPedidos _repositorioPedidos;

    public PedidosController(IServicioPedidos servicioPedidos, IRepositorioPedidos repositorioPedidos)
    {
        _servicioPedidos = servicioPedidos;
        _repositorioPedidos = repositorioPedidos;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repositorioPedidos.GetAll());
    }

    [HttpPost]
    public IActionResult CrearPedido([FromBody] Pedido pedido)
    {
        var resultado = _servicioPedidos.CrearPedido(pedido);
        if (!resultado)
        {
            return BadRequest("No se pudo crear el pedido. Verifique los datos ingresados.");
        }
        return Ok(resultado);
    }

    [HttpPut("{id:guid}/confirmar")]
    public IActionResult ConfirmarPedido(Guid id)
    {
        var resultado = _servicioPedidos.ConfirmarPedido(id);
        if (!resultado)
        {
            return BadRequest("No se pudo confirmar el pedido. Verifique que exista y esté pendiente.");
        }
        return Ok(resultado);
    }

    [HttpPut("{id:guid}/cancelar")]
    public IActionResult CancelarPedido(Guid id)
    {
        var resultado = _servicioPedidos.CancelarPedido(id);
        if (!resultado)
        {
            return BadRequest("No se pudo cancelar el pedido. Verifique que exista y esté pendiente.");
        }
        return Ok(resultado);
    }
}
