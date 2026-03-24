using System;
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
    public class WaterCompaniesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public WaterCompaniesController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaterCompany>>> GetWaterCompany()
        {
            return await _context.WaterCompany.OrderByDescending(p => p.id).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WaterCompany>> GetWaterCompany(long id)
        {
            var entity = await _context.WaterCompany.FindAsync(id);
            if (entity == null)
                return NotFound();
            return entity;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWaterCompany(long id, WaterCompany waterCompany)
        {
            if (id != waterCompany.id)
                return BadRequest();

            _context.Entry(waterCompany).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterCompanyExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<WaterCompany>> PostWaterCompany(WaterCompany waterCompany)
        {
            if (waterCompany.id == 0)
                _context.WaterCompany.Add(waterCompany);
            else
                _context.WaterCompany.Update(waterCompany);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWaterCompany", new { id = waterCompany.id }, waterCompany);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<WaterCompany>> DeleteWaterCompany(long id)
        {
            var waterCompany = await _context.WaterCompany.FindAsync(id);
            if (waterCompany == null)
                return NotFound();

            _context.WaterCompany.Remove(waterCompany);
            await _context.SaveChangesAsync();

            return waterCompany;
        }

        private bool WaterCompanyExists(long id)
        {
            return _context.WaterCompany.Any(e => e.id == id);
        }
    }
}
