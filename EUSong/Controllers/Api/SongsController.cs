using Microsoft.AspNetCore.Mvc;
using EUSong.Data;
using EUSong.Models;
using System.Collections.Generic;
using System.Linq;

namespace EUSong.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SongsController(AppDbContext context) => _context = context;

        // GET /api/songs
        [HttpGet]
        public ActionResult<IEnumerable<Song>> GetAll()
            => Ok(_context.Songs.ToList());
    }
}
