﻿using FeatureToggles.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureToggles.Api.Controllers
{
    /// <summary>
    /// Product controller for interacting with product entities
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly FeatureTogglesContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ProductsController(FeatureTogglesContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>A collection of products.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await this.context.Products.ToListAsync();
        }

        /// <summary>
        /// Gets a product by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A product.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await this.context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        /// <summary>
        /// Updates a product by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="product">The product.</param>
        /// <returns>A status code representing success or failure of update.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutProduct(Guid id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            this.context.Entry(product).State = EntityState.Modified;

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>The created product.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            this.context.Products.Add(product);
            await this.context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        /// <summary>
        /// Deletes a product by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The deleted product.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> DeleteProduct(Guid id)
        {
            var product = await this.context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            this.context.Products.Remove(product);
            await this.context.SaveChangesAsync();

            return product;
        }

        /// <summary>
        /// Products the exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A value indicating that the product exists or not.</returns>
        private bool ProductExists(Guid id)
        {
            return this.context.Products.Any(e => e.Id == id);
        }
    }
}