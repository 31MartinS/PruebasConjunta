using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GestionProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDBContext _appDbContext;

        public VentasController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // Obtener todas las ventas
        [HttpGet]
        public async Task<IActionResult> GetVentas()
        {
            var ventas = await _appDbContext.Ventas.Include(v => v.Producto).ToListAsync();
            return Ok(ventas);
        }

        // Crear una nueva venta
        [HttpPost]
        public async Task<IActionResult> CreateVenta([FromBody] Ventas venta)
        {
            if (venta == null)
                return BadRequest("La venta no puede ser nula.");

            // Validar que la fecha no sea menor a la actual
            if (venta.fecha_venta_total < DateTime.Now)
                return BadRequest("La fecha de la venta no puede ser menor a la fecha actual.");

            _appDbContext.Ventas.Add(venta);
            await _appDbContext.SaveChangesAsync();

            return Ok(venta);
        }

        // Editar una venta existente
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarVenta(int id, [FromBody] Ventas venta)
        {
            if (venta == null)
                return BadRequest("La venta no puede ser nula.");

            var ventaExistente = await _appDbContext.Ventas.FindAsync(id);
            if (ventaExistente == null)
                return NotFound("La venta no existe.");

            // Validar que la fecha no sea menor a la actual
            if (venta.fecha_venta_total < DateTime.Now)
                return BadRequest("La fecha de la venta no puede ser menor a la fecha actual.");

            // Actualizar propiedades
            ventaExistente.Cantidad = venta.Cantidad;
            ventaExistente.fecha_venta_total = venta.fecha_venta_total;
            ventaExistente.Total = venta.Total;

            await _appDbContext.SaveChangesAsync();

            return Ok(ventaExistente);
        }

        // Eliminar una venta por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarVenta(int id)
        {
            var ventaExistente = await _appDbContext.Ventas.FindAsync(id);
            if (ventaExistente == null)
                return NotFound("La venta no existe.");

            // Verificar si hay productos asociados a esta venta
            var productosAsociados = await _appDbContext.Productos.AnyAsync(p => p.Id == ventaExistente.ProductoId);
            if (productosAsociados)
                return BadRequest("No se puede eliminar la venta porque tiene productos asociados.");

            _appDbContext.Ventas.Remove(ventaExistente);
            await _appDbContext.SaveChangesAsync();

            return Ok($"Venta con ID {id} eliminada correctamente.");
        }
    }
}
