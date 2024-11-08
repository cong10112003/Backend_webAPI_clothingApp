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
    public class CustomersController : ApiController
    {
        private OnlineShopEntities db = new OnlineShopEntities();

        // GET: api/Customers
        [HttpGet]
        public async Task<List<Customer>> Get()
        {
            List<Customer> customers = await db.Customers.ToListAsync();

            return customers;
        }

        // GET: api/Customers/5
        [HttpGet]
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> Get(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerID)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        [HttpPost]
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerID }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete]
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> DeleteCustomer(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            await db.SaveChangesAsync();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerID == id) > 0;
        }
    }
}