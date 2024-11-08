using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication1.Models;

namespace WebApplication1.Controllers.WebAPI
{
    public class CategoriesController : ApiController
    {
        private OnlineShopEntities db = new OnlineShopEntities();

        // GET: api/Categories
        [HttpGet]
        public async Task<List<Category>> Get()
        {
            List<Category> categories = await db.Categories.ToListAsync();

            return categories;
        }

        // GET: api/Categories/5
        [HttpGet]
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> Get(int id)
        {
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCategory(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.CategoryID)
            {
                return BadRequest();
            }

            // Tìm danh mục hiện tại và bao gồm cả các sản phẩm hiện có
            var existingCategory = await db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            // Danh sách sản phẩm mới trong yêu cầu PUT
            var updatedProducts = category.Products.ToDictionary(p => p.ProductID);

            // Cập nhật các sản phẩm hiện có và giữ nguyên các sản phẩm không được gửi kèm
            foreach (var existingProduct in existingCategory.Products.ToList())
            {
                if (updatedProducts.TryGetValue(existingProduct.ProductID, out var updatedProduct))
                {
                    // Nếu sản phẩm đã tồn tại, cập nhật thuộc tính
                    existingProduct.ProductName = updatedProduct.ProductName;
                    existingProduct.Price = updatedProduct.Price;
                    existingProduct.Description = updatedProduct.Description;
                    existingProduct.Rate = updatedProduct.Rate;
                    existingProduct.ThumbNail = updatedProduct.ThumbNail;

                    // Đánh dấu là đã xử lý để tránh thêm lại sản phẩm này
                    updatedProducts.Remove(existingProduct.ProductID);
                }
            }

            // Thêm các sản phẩm mới chưa có trong danh mục
            foreach (var newProduct in updatedProducts.Values)
            {
                existingCategory.Products.Add(newProduct);
                db.Entry(newProduct).State = EntityState.Added;
            }

            // Đánh dấu danh mục đã thay đổi
            db.Entry(existingCategory).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        // POST: api/Categories
        [HttpPost]
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> PostCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Categories.Add(category);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = category.CategoryID }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete]
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            await db.SaveChangesAsync();

            return Ok(category);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return db.Categories.Count(e => e.CategoryID == id) > 0;
        }
    }
}