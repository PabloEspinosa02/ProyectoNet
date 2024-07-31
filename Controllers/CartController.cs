using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaUT.Context;
using TiendaUT.Domain;

namespace TiendaUT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems(int userId)
        {
            return await _context.CartItems.Include(ci => ci.Product)
                                           .Where(ci => ci.UserId == userId)
                                           .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<CartItem>> AddToCart(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCartItems), new { userId = cartItem.UserId }, cartItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
