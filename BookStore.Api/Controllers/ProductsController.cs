using BookStore.Api.Data;
using BookStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _databaseContext;

    public ProductsController(AppDbContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            List<Product> products =
                await _databaseContext.Products.ToListAsync();

            return Ok(products);
        }
        catch
        {
            return StatusCode(500);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        Product? product =
            await _databaseContext.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        _databaseContext.Products.Add(product);

        await _databaseContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        _databaseContext.Products.Update(product);

        await _databaseContext.SaveChangesAsync();

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        Product? product =
            await _databaseContext.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        _databaseContext.Products.Remove(product);

        await _databaseContext.SaveChangesAsync();

        return Ok();
    }
}