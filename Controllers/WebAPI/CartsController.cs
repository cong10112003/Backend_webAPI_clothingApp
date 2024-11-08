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
    public class CartsController : ApiController
    {
        private OnlineShopEntities db = new OnlineShopEntities();

        // GET: api/Carts
        [HttpGet]
        public async Task<List<Cart>> Get()
        {
            List<Cart> carts = await db.Carts.ToListAsync();

            return carts;
        }

        // GET: api/Carts/5
        [HttpGet]
        [ResponseType(typeof(Cart))]
        public async Task<IHttpActionResult> GetCart(int id)
        {
            Cart cart = await db.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        // PUT: api/Carts/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCart(int id, Cart cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cart.CartID)
            {
                return BadRequest();
            }

            db.Entry(cart).State = EntityState.Modified;

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

        // POST: api/Carts
        [HttpPost]
        [ResponseType(typeof(Cart))]
        public async Task<IHttpActionResult> PostCart(Cart cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Carts.Add(cart);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = cart.CartID }, cart);
        }

        // DELETE: api/Carts/5
        [HttpDelete]
        [ResponseType(typeof(Cart))]
        public async Task<IHttpActionResult> DeleteCart(int id)
        {
            Cart cart = await db.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            db.Carts.Remove(cart);
            await db.SaveChangesAsync();

            return Ok(cart);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CartExists(int id)
        {
            return db.Carts.Count(e => e.CartID == id) > 0;
        }
    }
}