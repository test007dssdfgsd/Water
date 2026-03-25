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
    public class WaterTumenController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public WaterTumenController(ApplicationContext context)
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

        // GET: api/WaterTumen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaterTuman>>> GetWaterTuman()
        {
            IQueryable<WaterTuman> query = _context.WaterTuman.AsQueryable();
            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null)
            {
                query = query.Where(p => p.company_id == tokenCompanyId);
            }
            return await query.ToListAsync();
        }

        // GET: api/WaterTumen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WaterTuman>> GetWaterTuman(long id)
        {
            var waterTuman = await _context.WaterTuman.FindAsync(id);

            if (waterTuman == null)
            {
                return NotFound();
            }

            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null && waterTuman.company_id != tokenCompanyId)
            {
                return NotFound();
            }

            return waterTuman;
        }

        [HttpGet("getPagination")]
        public async Task<ActionResult<JsonPaginationModel>> getPagination([FromQuery] int page, [FromQuery] int size)
        {
            JsonPaginationModel paginationModel = new JsonPaginationModel();
            IQueryable<WaterTuman> query = _context.WaterTuman
                .Include(p => p.viloyat)
                .Where(p => p.active_status == true);
            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null)
            {
                query = query.Where(p => p.company_id == tokenCompanyId);
            }
            List<WaterTuman> categoryList = await query.Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (categoryList == null)
            {
                categoryList = new List<WaterTuman>();
            }
            paginationModel.items_list = JArray.FromObject(categoryList);
            paginationModel.items_count = await query.CountAsync();
            paginationModel.current_item_count = categoryList.Count();
            paginationModel.current_page = page;
            return paginationModel;
        }

        // PUT: api/WaterTumen/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWaterTuman(long id, WaterTuman waterTuman)
        {
            if (id != waterTuman.id)
            {
                return BadRequest();
            }

            _context.Entry(waterTuman).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterTumanExists(id))
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

        // POST: api/WaterTumen
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<WaterTuman>> PostWaterTuman(WaterTuman waterTuman)
        {
            long? tokenCompanyId = GetTokenCompanyId();
            if (tokenCompanyId != null)
            {
                waterTuman.company_id = tokenCompanyId;
            }
            if (waterTuman.company_id == null || waterTuman.company_id <= 0)
            {
                return BadRequest("company_id is required");
            }

            long? tokenWaterUserId = GetTokenWaterUserId();
            if (tokenWaterUserId != null)
            {
                waterTuman.reserverd_numeric_id_3 = Convert.ToDouble(tokenWaterUserId.Value);
            }

            _context.WaterTuman.Update(waterTuman);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWaterTuman", new { id = waterTuman.id }, waterTuman);
        }

        // DELETE: api/WaterTumen/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WaterTuman>> DeleteWaterTuman(long id)
        {
            var waterTuman = await _context.WaterTuman.FindAsync(id);
            if (waterTuman == null)
            {
                return NotFound();
            }

            _context.WaterTuman.Remove(waterTuman);
            await _context.SaveChangesAsync();

            return waterTuman;
        }

        private bool WaterTumanExists(long id)
        {
            return _context.WaterTuman.Any(e => e.id == id);
        }
    }
}
