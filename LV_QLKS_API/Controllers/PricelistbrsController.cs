using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShareModel;
using ShareModel.Custom;

namespace LV_QLKS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricelistbrsController : ControllerBase
    {
        private readonly LV_QLKSContext _context;

        public PricelistbrsController(LV_QLKSContext context)
        {
            _context = context;
        }

        // GET: api/Pricelistbrs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pricelistbr>>> GetPricelistbrs()
        {
            return await _context.Pricelistbrs.ToListAsync();
        }

        // GET: api/Pricelistbrs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pricelistbr>> GetPricelistbr(int id)
        {
            var pricelistbr = await _context.Pricelistbrs.FindAsync(id);

            if (pricelistbr == null)
            {
                return NotFound();
            }

            return pricelistbr;
        }

        // PUT: api/Pricelistbrs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPricelistbr(int id, PriceListBr_Custom pricelistbr)
        {
            if (id != pricelistbr.PricelistbrId)
            {
                return BadRequest();
            }
            var pricelistbrTemp = new Pricelistbr();
            pricelistbrTemp.PricelistbrId = id;
            pricelistbrTemp.PricelistbrPrice = pricelistbr.PricelistbrPrice;
            pricelistbrTemp.PricelistbrName = pricelistbr.PricelistbrName;
            pricelistbrTemp.PricelistbrMonth = pricelistbr.PricelistbrMonth;
            _context.Entry(pricelistbrTemp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricelistbrExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPricelistbr", new { id = pricelistbrTemp.PricelistbrId }, pricelistbrTemp);
        }

        // POST: api/Pricelistbrs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pricelistbr>> PostPricelistbr(PriceListBr_Custom pricelistbr)
        {
            var pricelistbrTemp = new Pricelistbr();
            pricelistbrTemp.PricelistbrPrice = pricelistbr.PricelistbrPrice;
            pricelistbrTemp.PricelistbrName = pricelistbr.PricelistbrName;
            pricelistbrTemp.PricelistbrMonth = pricelistbr.PricelistbrMonth;
            _context.Pricelistbrs.Add(pricelistbrTemp);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPricelistbr", new { id = pricelistbrTemp.PricelistbrId }, pricelistbrTemp);
        }

        // DELETE: api/Pricelistbrs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricelistbr(int id)
        {
            var pricelistbr = await _context.Pricelistbrs.FindAsync(id);
            if (pricelistbr == null)
            {
                return NotFound();
            }

            _context.Pricelistbrs.Remove(pricelistbr);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PricelistbrExists(int id)
        {
            return _context.Pricelistbrs.Any(e => e.PricelistbrId == id);
        }
    }
}
