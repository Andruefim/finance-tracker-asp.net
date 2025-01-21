using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Data;
using AngularWithASP.Server.Models;
using AngularWithASP.Server.Services;
using System.Security.Claims;

namespace AngularWithASP.Server.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(
            ApplicationDbContext context,
            ICategoriesService categoriesService
        ) {
            _context = context;
            _categoriesService = categoriesService;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            var userId = User.FindFirstValue("UserId");
            var categories = await _categoriesService.GetCategoriesAsync(userId);

            return Ok(categories);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(long id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(long id, Category category)
        {
            var userId = User.FindFirstValue("UserId");

            var updatedCategory = await _categoriesService.UpdateCategoryAsync(id, userId, category);

            if (updatedCategory == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            var userId = User.FindFirstValue("UserId");

            var createdCategory = await _categoriesService.AddCategoryAsync(userId, category);

            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            var success = await _categoriesService.DeleteCategoryAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
