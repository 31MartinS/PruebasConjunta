using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace GestionProductos
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public required DbSet<Producto> Productos { get; set; }
        public required DbSet<Categoria> Categorias { get; set; }
        public required DbSet<Ventas> Ventas { get; set; }
        public required DbSet<Clientes> Clientes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Generar ID atuomaticos
            modelBuilder.Entity<Producto>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Categoria>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Ventas>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Clientes>().Property(p => p.Id).ValueGeneratedOnAdd();

            //Relaciones
            modelBuilder.Entity<Producto>().HasOne<Categoria>().WithMany().HasForeignKey(p => p.CategoriaId);
            modelBuilder.Entity<Ventas>().HasOne<Producto>().WithMany().HasForeignKey(p => p.ProductoId);

        }
    }
    public class Producto
    {
        [JsonRequired]
        public int Id { get; set; }
        [JsonRequired]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "La descripcion solo puede contener letras y espacios")]
        [StringLength(50, ErrorMessage = "La descripcion no debe exceder los 50 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio por noche es obligatorio.")]
        [RegularExpression(@"^\d+([.,]\d+)?$", ErrorMessage = "El precio por noche debe contener solo números")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio por noche debe ser mayor a 0.")]
        public decimal? Precio { get; set; }

        [Required(ErrorMessage = "La cantidad de stock es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de stock debe ser mayor a 0.")]
        public int? Stock { get; set; }
    }
    public class Categoria
    {
        [JsonRequired]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "La descripcion solo puede contener letras y espacios")]
        [StringLength(50, ErrorMessage = "La descripcion no debe exceder los 50 caracteres.")]
        public required string Descripcion { get; set; }    
    }

    public class Ventas
    {
        [JsonRequired]
        public int Id { get; set; }
        [JsonRequired]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public required int? Cantidad { get; set; }
        [Required(ErrorMessage = "La fecha de venta es obligatoria.")]
        public required DateTime? fecha_venta_total { get; set; }
        [Required(ErrorMessage = "El total es obligatorio.")]
        [RegularExpression(@"^\d+([.,]\d+)?$", ErrorMessage = "El total debe contener solo números")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0.")]
        public required decimal? Total { get; set; }

        [JsonIgnore]
        public Producto? Producto { get; set; }

    }

    public class Clientes
    {
        [JsonRequired]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres.")]
        public required string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios")]
        [StringLength(50, ErrorMessage = "El apellido no debe exceder los 50 caracteres.")]
        public required string? Apellido { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no es valido.")]
        public required string? Email { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El teléfono debe contener exactamente 10 dígitos.")]
        public required string? Telefono { get; set; }
    }
}