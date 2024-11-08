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
    public class OrderProductsController : ApiController
    {
        private OnlineShopEntities db = new OnlineShopEntities();

        // GET: api/OrderProducts
        [HttpGet]
        public async Task<List<OrderProduct>> Get()
        {
            List<OrderProduct> orderProducts = await db.OrderProducts.ToListAsync();

            return orderProducts;
        }

        // GET: api/OrderProducts/5
        [HttpGet]
        [ResponseType(typeof(OrderProduct))]
        public async Task<IHttpActionResult> GetOrderProduct(int id)
        {
            OrderProduct orderProduct = await db.OrderProducts.FindAsync(id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            return Ok(orderProduct);
        }

        // PUT: api/OrderProducts/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOrderProduct(int id, OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderProduct.OrderID)
            {
                return BadRequest();
            }

            db.Entry(orderProduct).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderProductExists(id))
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

        // POST: api/OrderProducts
        [HttpPost]
        [ResponseType(typeof(OrderProduct))]
        public async Task<IHttpActionResult> PostOrderProduct(OrderProduct orderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderProducts.Add(orderProduct);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderProductExists(orderProduct.OrderID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = orderProduct.OrderID }, orderProduct);
        }

        // DELETE: api/OrderProducts/5
        [HttpDelete]
        [ResponseType(typeof(OrderProduct))]
        public async Task<IHttpActionResult> DeleteOrderProduct(int id)
        {
            OrderProduct orderProduct = await db.OrderProducts.FindAsync(id);
            if (orderProduct == null)
            {
                return NotFound();
            }

            db.OrderProducts.Remove(orderProduct);
            await db.SaveChangesAsync();

            return Ok(orderProduct);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderProductExists(int id)
        {
            return db.OrderProducts.Count(e => e.OrderID == id) > 0;
        }
    }
}