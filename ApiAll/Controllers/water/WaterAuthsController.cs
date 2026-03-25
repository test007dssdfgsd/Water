using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ApiAll.Contex;
using ApiAll.Model.water;

namespace ApiAll.Controllers.water
{
    [ApiExplorerSettings(GroupName = "v9")]
    [Route("api/[controller]")]
    [ApiController]
    public class WaterAuthsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;

        public WaterAuthsController(ApplicationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/WaterAuths
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaterAuth>>> GetWaterAuth()
        {
            return await _context.WaterAuth.ToListAsync();
        }

        // GET: api/WaterAuths/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WaterAuth>> GetWaterAuth(long id)
        {
            var waterAuth = await _context.WaterAuth.FindAsync(id);

            if (waterAuth == null)
            {
                return NotFound();
            }

            return waterAuth;
        }

        [HttpGet("addOrderIdListForAuth")]
        public async Task<ActionResult<WaterAuth>> addOrderIdListForAuth([FromQuery]long auth_id,
            [FromQuery]String id_str_list)
        {
            var waterAuth = await _context.WaterAuth.FindAsync(auth_id);

            if (waterAuth == null)
            {
                return NotFound();
            }
            waterAuth.reserverd_note = id_str_list;
            _context.WaterAuth.Update(waterAuth);
            await _context.SaveChangesAsync();
            return waterAuth;
        }

        // GET: api/WaterAuths/5
        [HttpGet("checkAuth")]
        public async Task<ActionResult<object>> checkAuth([FromQuery] String login,[FromQuery] String password)
        {
            var waterAuth = await _context.WaterAuth
                .Where(p => p.password == password && p.login == login)
                .ToListAsync();

            if (waterAuth == null || waterAuth.Count() == 0)
            {
                return NotFound();
            }

            WaterAuth auth = waterAuth.First();
            var claims = new List<Claim>
            {
                new Claim("auth_id", auth.id.ToString()),
                new Claim("company_id", (auth.company_id ?? 0).ToString()),
                new Claim("water_user_id", auth.WaterUserid.ToString()),
                new Claim("user_type", auth.user_type.ToString()),
                new Claim(ClaimTypes.Name, auth.login ?? "")
            };

            var keyString = _configuration["Jwt:Key"];
            if (String.IsNullOrWhiteSpace(keyString))
            {
                keyString = "WATER_DEFAULT_SUPER_SECRET_KEY_2026_CHANGE_ME";
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenValue,
                id = auth.id,
                login = auth.login,
                user_type = auth.user_type,
                company_id = auth.company_id,
                waterUserid = auth.WaterUserid
            });
        }



        // PUT: api/WaterAuths/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWaterAuth(long id, WaterAuth waterAuth)
        {
            if (id != waterAuth.id)
            {
                return BadRequest();
            }

            _context.Entry(waterAuth).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterAuthExists(id))
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

        // POST: api/WaterAuths
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<WaterAuth>> PostWaterAuth(WaterAuth waterAuth)
        {
            if (waterAuth.company_id == null || waterAuth.company_id <= 0)
            {
                var claimCompany = User?.Claims?.FirstOrDefault(c => c.Type == "company_id")?.Value;
                if (!String.IsNullOrWhiteSpace(claimCompany) && long.TryParse(claimCompany, out long tokenCompanyId) && tokenCompanyId > 0)
                {
                    waterAuth.company_id = tokenCompanyId;
                }
            }

            // Legacy pages may not send company_id; fallback from related WaterUser.
            if ((waterAuth.company_id == null || waterAuth.company_id <= 0) && waterAuth.WaterUserid > 0)
            {
                var relatedUser = await _context.WaterUser.FindAsync(waterAuth.WaterUserid);
                if (relatedUser != null && relatedUser.company_id != null && relatedUser.company_id > 0)
                {
                    waterAuth.company_id = relatedUser.company_id;
                }
            }

            if (waterAuth.company_id == null || waterAuth.company_id <= 0)
            {
                return BadRequest("company_id is required");
            }

            _context.WaterAuth.Update(waterAuth);
            await _context.SaveChangesAsync();
            return waterAuth;
        }

        // DELETE: api/WaterAuths/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WaterAuth>> DeleteWaterAuth(long id)
        {
            var waterAuth = await _context.WaterAuth.FindAsync(id);
            if (waterAuth == null)
            {
                return NotFound();
            }

            _context.WaterAuth.Remove(waterAuth);
            await _context.SaveChangesAsync();

            return waterAuth;
        }

        private bool WaterAuthExists(long id)
        {
            return _context.WaterAuth.Any(e => e.id == id);
        }
    }
}
