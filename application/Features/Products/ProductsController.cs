using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EdAppTest.Features.Products
{
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        
        public ProductsController(IProductService productService)
        {
            this.productService = productService; 
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = productService.AddProduct(new EdAppTest.Domain.Product
            {
                Name = request.ProductName                
            });

            return CreatedAtAction(nameof(Get), new { product.Id }, new ProductViewModel(product));
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            // TODO: implememt get product
            return Ok();
        }

        public class CreateProductRequest
        {
            [Required]
            [MinLength(1)]
            public string ProductName { get; set; }
        }
        
    }
}
