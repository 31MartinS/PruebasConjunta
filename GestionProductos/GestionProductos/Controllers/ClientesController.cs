using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GestionProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDBContext _appDbContext;

        public ClientesController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // Obtener todos los clientes
        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _appDbContext.Clientes.ToListAsync();
            return Ok(clientes);
        }

        // Crear un nuevo cliente
        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] Clientes cliente)
        {
            if (cliente == null)
                return BadRequest("El cliente no puede ser nulo.");

            // Validar que no exista otro cliente con el mismo email
            var clienteExistente = await _appDbContext.Clientes
                .FirstOrDefaultAsync(c => string.Equals(c.Email, cliente.Email, StringComparison.OrdinalIgnoreCase));
            if (clienteExistente != null)
                return BadRequest("Ya existe un cliente con el mismo email.");

            _appDbContext.Clientes.Add(cliente);
            await _appDbContext.SaveChangesAsync();

            return Ok(cliente); // Devuelve el cliente creado con 200 OK
        }

        // Editar un cliente existente
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCliente(int id, [FromBody] Clientes cliente)
        {
            if (cliente == null)
                return BadRequest("El cliente no puede ser nulo.");

            var clienteExistente = await _appDbContext.Clientes.FindAsync(id);
            if (clienteExistente == null)
                return NotFound("El cliente no existe.");

            // Validar que no exista otro cliente con el mismo email
            var otroClienteConMismoEmail = await _appDbContext.Clientes
                .FirstOrDefaultAsync(c => string.Equals(c.Email, cliente.Email, StringComparison.OrdinalIgnoreCase) && c.Id != id);
            if (otroClienteConMismoEmail != null)
                return BadRequest("Ya existe otro cliente con el mismo email.");

            // Actualizar propiedades
            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.Apellido = cliente.Apellido;
            clienteExistente.Email = cliente.Email;
            clienteExistente.Telefono = cliente.Telefono;

            await _appDbContext.SaveChangesAsync();

            return Ok(clienteExistente); // Devuelve el cliente actualizado con 200 OK
        }

        // Eliminar un cliente por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var clienteExistente = await _appDbContext.Clientes.FindAsync(id);
            if (clienteExistente == null)
                return NotFound("El cliente no existe.");

            _appDbContext.Clientes.Remove(clienteExistente);
            await _appDbContext.SaveChangesAsync();

            return Ok($"Cliente con ID {id} eliminado correctamente.");
        }
    }
}
