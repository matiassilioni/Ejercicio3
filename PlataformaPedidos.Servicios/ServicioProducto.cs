using PlataformaPedidos.Dominio.Contratos;
using PlataformaPedidos.Dominio.Contratos.Persistencia;
using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Servicios;

public class ServicioProducto : IServicioProducto
{
    private IRepositorioProductos _repositorioProductos;

    public ServicioProducto(IRepositorioProductos repositorioProductos)
    {
        _repositorioProductos = repositorioProductos;
    }

    public Producto CrearProducto(Producto producto)
    {
        if (producto == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(producto.Nombre))
        {
            return null;
        }

        if (string.IsNullOrEmpty(producto.Descripcion))
        {
            return null;
        }

        if (producto.Precio <= 0)
        {
            return null;
        }

        producto.Id = Guid.NewGuid();

        bool graboBien = _repositorioProductos.SaveOrUpdateProducto(producto);

        if (!graboBien)
        {
            return null;
        }

        return producto;
    }
}
