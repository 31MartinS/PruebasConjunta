using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GestionProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDBContext _appDbContext;

        public ProductosController(AppDBContext appDBContext)
        {
            _appDbContext = appDBContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _appDbContext.Productos.ToListAsync();
            return Ok(productos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProducto([FromBody] Producto producto)
        {
            if (producto == null)
                return BadRequest("El producto no puede ser nulo.");
            if (producto.Precio < 0)
                return BadRequest("El precio del producto no puede ser negativo.");
            if (producto.Stock < 0)
                return BadRequest("El stock del producto no puede ser negativo.");

            // Validar que no exista otro producto con el mismo nombre
            var existeProducto = await _appDbContext.Productos
                .FirstOrDefaultAsync(p => string.Equals(p.Nombre, producto.Nombre, StringComparison.OrdinalIgnoreCase));
            if (existeProducto != null)
                return BadRequest("Ya existe un producto con el mismo nombre.");

            _appDbContext.Productos.Add(producto);
            await _appDbContext.SaveChangesAsync();

            return Ok(producto); // Devolver el producto creado
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProducto(int id, [FromBody] Producto producto)
        {
            if (producto == null)
                return BadRequest("El producto no puede ser nulo.");
            if (producto.Precio < 0)
                return BadRequest("El precio del producto no puede ser negativo.");
            if (producto.Stock < 0)
                return BadRequest("El stock del producto no puede ser negativo.");

            var productoExistente = await _appDbContext.Productos.FindAsync(id);
            if (productoExistente == null)
                return NotFound("El producto no existe.");

            // Validar que no exista otro producto con el mismo nombre
            var existeOtroProducto = await _appDbContext.Productos
                .FirstOrDefaultAsync(p => string.Equals(p.Nombre, producto.Nombre, StringComparison.OrdinalIgnoreCase) && p.Id != id);
            if (existeOtroProducto != null)
                return BadRequest("Ya existe un producto con el mismo nombre.");

            // Actualizar propiedades
            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock;

            await _appDbContext.SaveChangesAsync();

            return Ok(productoExistente); // Devolver el producto actualizado
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var productoExistente = await _appDbContext.Productos.FindAsync(id);
            if (productoExistente == null)
                return NotFound("El producto no existe.");

            _appDbContext.Productos.Remove(productoExistente);
            await _appDbContext.SaveChangesAsync();

            return Ok($"Producto con ID {id} eliminado correctamente.");
        }
    }
}
