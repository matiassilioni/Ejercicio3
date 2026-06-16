namespace PlataformaPedidos.Dominio.Enumeraciones;

/// <summary>
/// Representa el estado actual de un pedido en el sistema.
/// </summary>
public enum EstadoPedido
{
    /// <summary>
    /// El pedido ha sido creado pero aún no confirmado.
    /// </summary>
    Pendiente = 0,

    /// <summary>
    /// El pedido ha sido confirmado y está en procesamiento.
    /// </summary>
    Confirmado = 1,

    /// <summary>
    /// El pedido ha sido cancelado por el cliente o el sistema.
    /// </summary>
    Cancelado = 2
}
