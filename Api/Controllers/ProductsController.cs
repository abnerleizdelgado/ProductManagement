
using Domain.DTOs;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _productService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO product)
        {
            await _productService.AddAsync(product);
            return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDTO productDTO)
        {
            if (id != productDTO.Id) return BadRequest("ID mismatch.");
            await _productService.UpdateAsync(productDTO);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}