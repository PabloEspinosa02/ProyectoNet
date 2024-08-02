using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaUT.Context;
using TiendaUT.Domain;

namespace TiendaUT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductDto product)
        {
            Product newProduct = new Product()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Sizes = product.Sizes
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
            return Ok( newProduct);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> PutProduct(int id, ProductDto product)
        {

            try 
            {
                Product pro = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

                if (pro != null)
                {
                    pro.Name = product.Name;
                    pro.Description = product.Description;
                    pro.Price = product.Price;
                    pro.ImageUrl = product.ImageUrl;
                    pro.Sizes = product.Sizes;
                    _context.SaveChanges();
                }

                Product newProduct = new Product()
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Sizes = product.Sizes
                };

                _context.Products.Update(pro);
                await _context.SaveChangesAsync();

                return Ok(newProduct);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrio un error al actualizar" + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class ProductDto
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public List<string>? Sizes { get; set; } // Lista opcional de tallas
    }
}
