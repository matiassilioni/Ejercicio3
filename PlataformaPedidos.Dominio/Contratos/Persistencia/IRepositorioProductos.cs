using PlataformaPedidos.Dominio.Modelos;

namespace PlataformaPedidos.Dominio.Contratos.Persistencia;

public interface IRepositorioProductos
{
    public Producto GetById(Guid id);
    public List<Producto> GetAll();
    public bool SaveOrUpdateProducto(Producto producto);
}