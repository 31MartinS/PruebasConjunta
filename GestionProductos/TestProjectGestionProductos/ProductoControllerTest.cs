using GestionProductos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestProjectGestionProductos
{
    public class ProductoControllerTest
    {
        private AppDBContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;
            return new AppDBContext(options);
        }

        [Fact]
        public void Test1()
        {
            //Configurar
            var producto = new Producto { Nombre = "Prueba", Precio = -10 };
            //Validar
            Assert.True(producto.Precio >= 0, "El precio no puede ser negativo");
        }

        [Fact]
        public void Producto_DebeCrearseConNombreValido()
        {
            // Arrange
            var producto = new Producto { Nombre = "Test", CategoriaId = 1 };

            // Assert
            Assert.NotNull(producto.Nombre);
            Assert.NotEmpty(producto.Nombre);
        }

        [Fact]
        public async Task Producto_PuedeSerGuardadoEnBaseDeDatos()
        {
            // Arrange
            using var context = GetDbContext();
            var producto = new Producto
            {
                Nombre = "Producto Test",
                CategoriaId = 1,
                Precio = 100,
                Stock = 10
            };

            // Act
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            // Assert
            var savedProduct = await context.Productos.FindAsync(producto.Id);
            Assert.NotNull(savedProduct);
            Assert.Equal("Producto Test", savedProduct.Nombre);
        }

        [Theory]
        [InlineData("", 100)]
        [InlineData(null, 100)]
        [InlineData("Producto", -50)]
        public void Producto_ValidacionesBasicas(string nombre, decimal precio)
        {
            // Arrange & Act
            var producto = new Producto
            {
                Nombre = nombre,
                Precio = precio
            };

            // Assert
            if (string.IsNullOrEmpty(nombre))
            {
                Assert.False(string.IsNullOrEmpty(producto.Nombre), "El nombre no puede estar vac�o");
            }
            if (precio < 0)
            {
                Assert.True(producto.Precio >= 0, "El precio no puede ser negativo");
            }
        }
    }

    public class CategoriaControllerTest
    {
        private AppDBContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;
            return new AppDBContext(options);
        }

        [Fact]
        public async Task Categoria_PuedeSerCreada()
        {
            // Arrange
            using var context = GetDbContext();
            var categoria = new Categoria
            {
                Nombre = "Categor�a Test"
            };

            // Act
            context.Categorias.Add(categoria);
            await context.SaveChangesAsync();

            // Assert
            var savedCategory = await context.Categorias.FindAsync(categoria.Id);
            Assert.NotNull(savedCategory);
            Assert.Equal("Categor�a Test", savedCategory.Nombre);
        }

        [Fact]
        public async Task Categoria_PuedeContenerProductos()
        {
            // Arrange
            using var context = GetDbContext();
            var categoria = new Categoria { Nombre = "Categor�a Test" };
            context.Categorias.Add(categoria);
            await context.SaveChangesAsync();

            var producto = new Producto
            {
                Nombre = "Producto Test",
                CategoriaId = categoria.Id,
                Precio = 100
            };
            context.Productos.Add(producto);
            await context.SaveChangesAsync();

            // Act
            var savedProduct = await context.Productos
                .FirstOrDefaultAsync(p => p.CategoriaId == categoria.Id);

            // Assert
            Assert.NotNull(savedProduct);
            Assert.Equal(categoria.Id, savedProduct.CategoriaId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Categoria_NombreNoDebeSerNuloOVacio(string nombre)
        {
            // Arrange & Act
            var categoria = new Categoria { Nombre = nombre };

            // Assert
            Assert.False(string.IsNullOrEmpty(categoria.Nombre),
                "El nombre de la categor�a no puede estar vac�o");
        }
    }
}