using HSport.App.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HSport.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        public readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;
            
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task< IEnumerable<Product> > GetAllProducts ()
        {
            if (_context.Products == null)
            {
                return Enumerable.Empty<Product>();
            }

            return  _context.Products.ToArray();
        }

        /* Alternative way for getting  the 
            status code together with the products (Use Action Result)

        [HttpGet]
        public ActionResult GetAllProducts()
        {
            return Ok(_context.Products.ToArray());
        }
        */


        [HttpGet("{id}")]
        public async Task<ActionResult> GetOneProduct(int id)
        {
            // Retrieve the product with the given ID from the database asynchronously
            var product = await _context.Products.FindAsync(id);

            // Check if the product exists
            if (product == null)
            {
                // If the product does not exist, return a 404 Not Found status with a message
                return NotFound("Product with that Id Not Found");
            }

            // If the product exists, return it with a 200 OK status
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {
            
            // Check if a product with the same ID already exists
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct != null)
            {
                // If a product with the same ID already exists, return a conflict response
                return Conflict("Product with the same ID already exists");
            }

            
            // If the product does not exist, proceed to add and save the new product
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Return a 201 Created response with the newly created product
            return CreatedAtAction(
                "GetOneProduct",
                new { id = product.Id },
                product
            );
        }




    }
}
