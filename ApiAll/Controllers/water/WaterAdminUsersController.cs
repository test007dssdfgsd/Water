using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiAll.Contex;
using ApiAll.Model;
using ApiAll.Model.water;
using Newtonsoft.Json.Linq;

namespace ApiAll.Controllers.water
{
    [ApiExplorerSettings(GroupName = "v9")]
    [Route("api/[controller]")]
    [ApiController]
    public class WaterAdminUsersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public WaterAdminUsersController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaterAdminUser>>> GetWaterAdminUser()
        {
            List<WaterAdminUser> users = await _context.WaterAdminUser.ToListAsync();
            foreach (WaterAdminUser it in users)
            {
                List<WaterAdminAuth> auths = await _context.WaterAdminAuth.Where(p => p.WaterAdminUserid == it.id).ToListAsync();
                if (auths != null && auths.Count > 0)
                {
                    it.auth = auths.First();
                    it.auth_id = auths.First().id;
                }
            }

            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WaterAdminUser>> GetWaterAdminUser(long id)
        {
            var waterAdminUser = await _context.WaterAdminUser.FindAsync(id);
            if (waterAdminUser == null)
                return NotFound();
            return waterAdminUser;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWaterAdminUser(long id, WaterAdminUser waterAdminUser)
        {
            if (id != waterAdminUser.id)
                return BadRequest();

            _context.Entry(waterAdminUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterAdminUserExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<WaterAdminUser>> PostWaterAdminUser(WaterAdminUser waterAdminUser)
        {
            if (waterAdminUser.id == 0)
                _context.WaterAdminUser.Add(waterAdminUser);
            else
                _context.WaterAdminUser.Update(waterAdminUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWaterAdminUser", new { id = waterAdminUser.id }, waterAdminUser);
        }

        [HttpGet("getPagination")]
        public async Task<ActionResult<JsonPaginationModel>> getPagination([FromQuery] int page, [FromQuery] int size)
        {
            JsonPaginationModel paginationModel = new JsonPaginationModel();
            List<WaterAdminUser> list = await _context.WaterAdminUser
                .Where(p => p.active_status == true)
                .Skip(page * size).Take(size).OrderByDescending(p => p.id).ToListAsync();
            if (list == null)
                list = new List<WaterAdminUser>();
            paginationModel.items_list = JArray.FromObject(list);
            paginationModel.items_count = await _context.WaterAdminUser.Where(p => p.active_status == true).CountAsync();
            paginationModel.current_item_count = list.Count;
            paginationModel.current_page = page;
            return paginationModel;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<WaterAdminUser>> DeleteWaterAdminUser(long id)
        {
            var waterAdminUser = await _context.WaterAdminUser.FindAsync(id);
            if (waterAdminUser == null)
                return NotFound();

            _context.WaterAdminUser.Remove(waterAdminUser);
            await _context.SaveChangesAsync();

            return waterAdminUser;
        }

        private bool WaterAdminUserExists(long id)
        {
            return _context.WaterAdminUser.Any(e => e.id == id);
        }
    }
}
