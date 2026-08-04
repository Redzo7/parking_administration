using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.WebAPI.Data;
using Parking.WebAPI.DTOs;

namespace Parking.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(u => new UserResponseDTO
                {
                    Id = u.Id,
                    Name = u.Name
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}