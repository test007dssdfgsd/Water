using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiAll.Contex;
using ApiAll.Model.water;

namespace ApiAll.Controllers.water
{
    [ApiExplorerSettings(GroupName = "v9")]
    [Route("api/[controller]")]
    [ApiController]
    public class WaterContragentsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public WaterContragentsController(ApplicationContext context)
        {
            _context = context;
        }

        private long? GetTokenCompanyId()
        {
            var claimCompany = User?.Claims?.FirstOrDefault(c => c.Type == "company_id")?.Value;
            if (!String.IsNullOrWhiteSpace(claimCompany) && long.TryParse(claimCompany, out long parsedCompanyId) && parsedCompanyId > 0)
            {
                return parsedCompanyId;
            }
            return null;
        }

        private long? GetTokenWaterUserId()
        {
            var claimUserId = User?.Claims?.FirstOrDefault(c => c.Type == "water_user_id")?.Value;
            if (!String.IsNullOrWhiteSpace(claimUserId) && long.TryParse(claimUserId, out long parsedUserId) && parsedUserId > 0)
            {
                return parsedUserId;
            }
            return null;
        }

        // GET: api/WaterContragents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaterContragent>>> GetWaterContragent()
        {
            IQueryable<WaterContragent> query = _context.WaterContragent.AsQueryable();
            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null)
            {
                query = query.Where(p => p.company_id == tokenCompanyId);
            }
            return await query.ToListAsync();
        }

        // GET: api/WaterContragents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WaterContragent>> GetWaterContragent(long id)
        {
            var waterContragent = await _context.WaterContragent.FindAsync(id);

            if (waterContragent == null)
            {
                return NotFound();
            }

            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null && waterContragent.company_id != tokenCompanyId)
            {
                return NotFound();
            }

            return waterContragent;
        }

        // PUT: api/WaterContragents/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWaterContragent(long id, WaterContragent waterContragent)
        {
            if (id != waterContragent.id)
            {
                return BadRequest();
            }

            _context.Entry(waterContragent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterContragentExists(id))
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

        // POST: api/WaterContragents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<WaterContragent>> PostWaterContragent(WaterContragent waterContragent)
        {
            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null)
            {
                waterContragent.company_id = tokenCompanyId;
            }
            if (waterContragent.company_id == null || waterContragent.company_id <= 0)
            {
                return BadRequest("company_id is required");
            }

            long? tokenWaterUserId = GetTokenWaterUserId();
            if (tokenWaterUserId != null)
            {
                waterContragent.reserverd_numeric_id_3 = Convert.ToDouble(tokenWaterUserId.Value);
            }

            _context.WaterContragent.Update(waterContragent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWaterContragent", new { id = waterContragent.id }, waterContragent);
        }

        // DELETE: api/WaterContragents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WaterContragent>> DeleteWaterContragent(long id)
        {
            var waterContragent = await _context.WaterContragent.FindAsync(id);
            if (waterContragent == null)
            {
                return NotFound();
            }

            _context.WaterContragent.Remove(waterContragent);
            await _context.SaveChangesAsync();

            return waterContragent;
        }

        private bool WaterContragentExists(long id)
        {
            return _context.WaterContragent.Any(e => e.id == id);
        }
    }
}
