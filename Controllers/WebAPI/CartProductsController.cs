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
    public class CartProductsController : ApiController
    {
        private OnlineShopEntities db = new OnlineShopEntities();

        // GET: api/CartProducts
        [HttpGet]
        public async Task<List<CartProduct>> Get()
        {
            List<CartProduct> cartProducts = await db.CartProducts.ToListAsync();

            return cartProducts;
        }

        // GET: api/CartProducts/5
        [HttpGet]
        [ResponseType(typeof(CartProduct))]
        public async Task<IHttpActionResult> GetCartProduct(int id)
        {
            CartProduct cartProduct = await db.CartProducts.FindAsync(id);
            if (cartProduct == null)
            {
                return NotFound();
            }

            return Ok(cartProduct);
        }

        // PUT: api/CartProducts/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCartProduct(int id, Cart cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cart.CartID)
            {
                return BadRequest();
            }

            // Tìm giỏ hàng hiện tại bao gồm cả các sản phẩm
            var existingCart = await db.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.CartID == id);

            if (existingCart == null)
            {
                return NotFound();
            }

            // Tạo một từ điển cho các sản phẩm được gửi trong yêu cầu PUT
            var updatedCartProducts = cart.CartProducts.ToDictionary(cp => cp.ProductID);

            // Duyệt qua các sản phẩm hiện có trong giỏ
            foreach (var existingCartProduct in existingCart.CartProducts.ToList())
            {
                if (updatedCartProducts.TryGetValue(existingCartProduct.ProductID, out var updatedCartProduct))
                {
                    // Cập nhật số lượng nếu sản phẩm đã có
                    existingCartProduct.Quantity = updatedCartProduct.Quantity;

                    // Loại bỏ sản phẩm khỏi danh sách cập nhật để tránh thêm lại
                    updatedCartProducts.Remove(existingCartProduct.ProductID);
                }
            }

            // Thêm các sản phẩm mới chưa có trong giỏ hàng
            foreach (var newCartProduct in updatedCartProducts.Values)
            {
                existingCart.CartProducts.Add(newCartProduct);
                db.Entry(newCartProduct).State = EntityState.Added;
            }

            // Đánh dấu giỏ hàng đã thay đổi
            db.Entry(existingCart).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
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
        private bool CartExists(int id)
        {
            return db.Carts.Any(c => c.CartID == id);
        }


        // POST: api/CartProducts
        [HttpPost]
        [ResponseType(typeof(CartProduct))]
        public async Task<IHttpActionResult> PostCartProduct(CartProduct cartProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CartProducts.Add(cartProduct);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartProductExist(cartProduct.CartID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cartProduct.CartID }, cartProduct);
        }

        // DELETE: api/CartProducts/5
        /*[HttpDelete]
        [ResponseType(typeof(CartProduct))]
        public async Task<IHttpActionResult> DeleteCartProduct(int id)
        {
            CartProduct cartProduct = await db.CartProducts.FindAsync(id);
            if (cartProduct == null)
            {
                return NotFound();
            }

            db.CartProducts.Remove(cartProduct);
            await db.SaveChangesAsync();

            return Ok(cartProduct);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/
        [HttpDelete]
        [ResponseType(typeof(CartProduct))]
        public async Task<IHttpActionResult> DeleteCartProduct(int cartId, int productId)
        {
            // Tìm CartProduct theo CartID và ProductID
            CartProduct cartProduct = await db.CartProducts
                                              .Where(cp => cp.CartID == cartId && cp.ProductID == productId)
                                              .FirstOrDefaultAsync();

            // Nếu không tìm thấy CartProduct, trả về NotFound
            if (cartProduct == null)
            {
                return NotFound();
            }

            // Xóa CartProduct khỏi cơ sở dữ liệu
            db.CartProducts.Remove(cartProduct);

            // Lưu thay đổi vào cơ sở dữ liệu
            await db.SaveChangesAsync();

            // Trả về CartProduct đã xóa
            return Ok(cartProduct);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Kiểm tra CartProduct tồn tại hay không với CartID và ProductID
        private bool CartProductExists(int cartId, int productId)
        {
            return db.CartProducts.Any(e => e.CartID == cartId && e.ProductID == productId);
        }


        private bool CartProductExist(int id)
        {
            return db.CartProducts.Count(e => e.CartID == id) > 0;
        }
    }
}