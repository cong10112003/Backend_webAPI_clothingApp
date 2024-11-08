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
    public class OrderHistoriesController : ApiController
    {
        private OnlineShopEntities db = new OnlineShopEntities();

        // GET: api/OrderHistories
        [HttpGet]
        public async Task<List<OrderHistory>> Get()
        {
            List<OrderHistory> orderHistories = await db.OrderHistories.ToListAsync();

            return orderHistories ;
        }

        // GET: api/OrderHistories/5
        [HttpGet]
        [ResponseType(typeof(OrderHistory))]
        public async Task<IHttpActionResult> GetOrderHistory(int id)
        {
            OrderHistory orderHistory = await db.OrderHistories.FindAsync(id);
            if (orderHistory == null)
            {
                return NotFound();
            }

            return Ok(orderHistory);
        }

        // PUT: api/OrderHistories/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOrderHistory(int id, OrderHistory orderHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderHistory.OrderID)
            {
                return BadRequest();
            }

            db.Entry(orderHistory).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderHistoryExists(id))
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

        // POST: api/OrderHistories
        [HttpPost]
        [ResponseType(typeof(OrderHistory))]
        public async Task<IHttpActionResult> PostOrderHistory(OrderHistory orderHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderHistories.Add(orderHistory);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = orderHistory.OrderID }, orderHistory);
        }

        // DELETE: api/OrderHistories/5
        [HttpDelete]
        [ResponseType(typeof(OrderHistory))]
        public async Task<IHttpActionResult> DeleteOrderHistory(int id)
        {
            OrderHistory orderHistory = await db.OrderHistories.FindAsync(id);
            if (orderHistory == null)
            {
                return NotFound();
            }

            db.OrderHistories.Remove(orderHistory);
            await db.SaveChangesAsync();

            return Ok(orderHistory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderHistoryExists(int id)
        {
            return db.OrderHistories.Count(e => e.OrderID == id) > 0;
        }
    }
}