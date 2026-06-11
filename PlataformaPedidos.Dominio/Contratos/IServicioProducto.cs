using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Dominio.Contratos;

public interface IServicioProducto
{
    public Producto CrearProducto(Producto producto);
}