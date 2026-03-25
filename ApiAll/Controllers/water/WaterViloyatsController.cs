using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiAll.Contex;
using ApiAll.Model.water;
using ApiAll.Model;
using Newtonsoft.Json.Linq;

namespace ApiAll.Controllers.water
{
    [ApiExplorerSettings(GroupName = "v9")]
    [Route("api/[controller]")]
    [ApiController]
    public class WaterViloyatsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public WaterViloyatsController(ApplicationContext context)
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

        // GET: api/WaterViloyats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaterViloyat>>> GetWaterViloyat()
        {
            IQueryable<WaterViloyat> query = _context.WaterViloyat.AsQueryable();
            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null)
            {
                query = query.Where(p => p.company_id == tokenCompanyId);
            }
            return await query.ToListAsync();
        }

        // GET: api/WaterViloyats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WaterViloyat>> GetWaterViloyat(long id)
        {
            var waterViloyat = await _context.WaterViloyat.FindAsync(id);

            if (waterViloyat == null)
            {
                return NotFound();
            }

            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null && waterViloyat.company_id != tokenCompanyId)
            {
                return NotFound();
            }

            return waterViloyat;
        }

        [HttpGet("getPagination")]
        public async Task<ActionResult<JsonPaginationModel>> getPagination([FromQuery] int page, [FromQuery] int size)
        {
            JsonPaginationModel paginationModel = new JsonPaginationModel();
            List<WaterViloyat> categoryList = await _context.WaterViloyat
                .Where(p => p.active_status == true)
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterViloyat>();
            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await _context.WaterViloyat.Where(p => p.active_status == true).CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }

        // PUT: api/WaterViloyats/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWaterViloyat(long id, WaterViloyat waterViloyat)
        {
            if (id != waterViloyat.id)
            {
                return BadRequest();
            }

            _context.Entry(waterViloyat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterViloyatExists(id))
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

        // POST: api/WaterViloyats
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<WaterViloyat>> PostWaterViloyat(WaterViloyat waterViloyat)
        {
            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null)
            {
                waterViloyat.company_id = tokenCompanyId;
            }
            if (waterViloyat.company_id == null || waterViloyat.company_id <= 0)
            {
                return BadRequest("company_id is required");
            }

            long? tokenWaterUserId = GetTokenWaterUserId();
            if (tokenWaterUserId != null)
            {
                waterViloyat.reserverd_numeric_id_3 = Convert.ToDouble(tokenWaterUserId.Value);
            }

            _context.WaterViloyat.Update(waterViloyat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWaterViloyat", new { id = waterViloyat.id }, waterViloyat);
        }

        // DELETE: api/WaterViloyats/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WaterViloyat>> DeleteWaterViloyat(long id)
        {
            var waterViloyat = await _context.WaterViloyat.FindAsync(id);
            if (waterViloyat == null)
            {
                return NotFound();
            }

            _context.WaterViloyat.Remove(waterViloyat);
            await _context.SaveChangesAsync();

            return waterViloyat;
        }

        private bool WaterViloyatExists(long id)
        {
            return _context.WaterViloyat.Any(e => e.id == id);
        }
    }
}
