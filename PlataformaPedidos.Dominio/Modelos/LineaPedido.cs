namespace PlataformaPedidos.Dominio.Modelos;

/// <summary>
/// Representa una línea individual dentro de un pedido.
/// </summary>
public class LineaPedido
{
    /// <summary>
    /// Identificador del producto asociado a esta línea.
    /// </summary>
    public Producto Producto { get; set; }

    /// <summary>
    /// Cantidad de unidades del producto.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario del producto en el momento del pedido.
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Subtotal de esta línea (cantidad × precio unitario).
    /// </summary>
    public decimal Subtotal { get; set; }
}