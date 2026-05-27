using PlataformaPedidos.Dominio.Enumeraciones;

namespace PlataformaPedidos.Dominio.Modelos;

/// <summary>
/// Representa un pedido de productos digitales realizado por un cliente.
/// </summary>
public class Pedido
{
    /// <summary>
    /// Identificador único del pedido.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador del cliente que realizó el pedido.
    /// </summary>
    public Cliente Cliente { get; set; }

    /// <summary>
    /// Lista de líneas de detalle del pedido.
    /// </summary>
    public List<LineaPedido> Detalles { get; set; } = new List<LineaPedido>();

    /// <summary>
    /// Estado actual del pedido.
    /// </summary>
    public EstadoPedido Estado { get; set; }

    /// <summary>
    /// Fecha y hora en que se creó el pedido.
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Fecha y hora de confirmación del pedido.
    /// </summary>
    public DateTime? FechaConfirmacion { get; set; }

    /// <summary>
    /// Importe total del pedido.
    /// </summary>
    public decimal Total { get; set; }
}