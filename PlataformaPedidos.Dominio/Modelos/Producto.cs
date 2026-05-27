using PlataformaPedidos.Dominio.Enumeraciones;

namespace PlataformaPedidos.Dominio.Modelos;

/// <summary>
/// Representa un producto digital disponible en la tienda.
/// </summary>
public class Producto
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Nombre del producto.
    /// </summary>
    public string Nombre { get; set; }

    /// <summary>
    /// Descripción detallada del producto.
    /// </summary>
    public string Descripcion { get; set; }

    /// <summary>
    /// Precio unitario del producto.
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Fecha y hora de ultima modificacion del registro.
    /// </summary>
    public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indica si el producto está disponible para la venta.
    /// </summary>
    public bool Disponible { get; set; }
}
