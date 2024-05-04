using HSport.App.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("/get-all-products")]
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


        [HttpGet("/get-one-product/{id}")]
        public async Task<ActionResult> GetOneProduct(int id)
        {
            // Retrieve the product with the given ID from the database asynchronously
            var product = await _context.Products.FindAsync(id);

            // Check if the product exists
            if (product == null)
            {
                // If the product does not exist, return a 404 Not Found status with a message
                return NotFound("Product With that Id Not Found");
            }

            // If the product exists, return it with a 200 OK status
            return Ok(product);
        }

        [HttpPost("/create-product")]
        public async Task<ActionResult> PostProduct(Product product)
        {
            
            // Check if a product with the same ID already exists
            var existingProduct = await _context.Products.FindAsync(product.Id);

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

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


        [HttpPut("/update-product/{id}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if(id != product.Id)
            {
                return BadRequest("Product with that Id does not Exist!");
            }

             _context.Entry(product).State = EntityState.Modified;

            try
            {
                
                await _context.SaveChangesAsync();

            } catch (DbUpdateConcurrencyException)
            {
                if(!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound("Product not fount");
                } else
                {
                    throw;
                }
            }

            return Ok("Product updated successfully!");
        }

        // Deleting one Item
        [HttpDelete("/delete-product/{id}")]
        public async Task<ActionResult<Product>> DeleteOneProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound("Product not found");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            //Create a class for the response
            var response = new ProductResponse
            {
                Message = "Product deleted successfully",
                Product = product
            };

            return Ok(response);

        }


        // Deleting Multiple Items
        [HttpPost("/Delete-multiple-items")]
        
        public async Task<ActionResult<Product>> DeleteMultipleProducts([FromQuery] int[] ids)
        {

            var products = new List<Product>();
            foreach (int id in ids)
            {
                var product = _context.Products.Find(id);
                // Check the idscone by one if they are in the products
                if (product == null)
                {
                    return NotFound("Product not found");
                }

                // if found Add the product to the list of products to be deleted
                products.Add(product);
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok("Products deleted Successfully");

        }



    }
}
