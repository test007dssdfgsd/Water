using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiAll.Contex;
using ApiAll.Model.water;

namespace ApiAll.Controllers.water
{
    [ApiExplorerSettings(GroupName = "v9")]
    [Route("api/[controller]")]
    [ApiController]
    public class WaterAdminAuthsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public WaterAdminAuthsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaterAdminAuth>>> GetWaterAdminAuth()
        {
            return await _context.WaterAdminAuth.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WaterAdminAuth>> GetWaterAdminAuth(long id)
        {
            var waterAdminAuth = await _context.WaterAdminAuth.FindAsync(id);
            if (waterAdminAuth == null)
                return NotFound();
            return waterAdminAuth;
        }

        [HttpGet("checkAuth")]
        public async Task<ActionResult<WaterAdminAuth>> checkAuth([FromQuery] string login, [FromQuery] string password)
        {
            var list = await _context.WaterAdminAuth
                .Where(p => p.password == password && p.login == login)
                .ToListAsync();

            if (list == null || list.Count == 0)
                return NotFound();

            return list.First();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWaterAdminAuth(long id, WaterAdminAuth waterAdminAuth)
        {
            if (id != waterAdminAuth.id)
                return BadRequest();

            _context.Entry(waterAdminAuth).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterAdminAuthExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<WaterAdminAuth>> PostWaterAdminAuth(WaterAdminAuth waterAdminAuth)
        {
            if (waterAdminAuth.id == 0)
                _context.WaterAdminAuth.Add(waterAdminAuth);
            else
                _context.WaterAdminAuth.Update(waterAdminAuth);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetWaterAdminAuth", new { id = waterAdminAuth.id }, waterAdminAuth);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<WaterAdminAuth>> DeleteWaterAdminAuth(long id)
        {
            var waterAdminAuth = await _context.WaterAdminAuth.FindAsync(id);
            if (waterAdminAuth == null)
                return NotFound();

            _context.WaterAdminAuth.Remove(waterAdminAuth);
            await _context.SaveChangesAsync();

            return waterAdminAuth;
        }

        private bool WaterAdminAuthExists(long id)
        {
            return _context.WaterAdminAuth.Any(e => e.id == id);
        }
    }
}
