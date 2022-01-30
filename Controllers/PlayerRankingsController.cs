using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRA_WebAPI.Context;
using PRA_WebAPI.Entities;
using PRA_WebAPI.ViewModels;

namespace PRA_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerRankingsController : ControllerBase
    {
        private readonly PRAQuizContext _context;
        private readonly IMapper _mapper;

        public PlayerRankingsController(PRAQuizContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/PlayerRankings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerRankingViewModel>>> GetPlayerRankings()
        {
            var playerRankings = await _context.PlayerRankings.ToListAsync();
            var playerRankingViewModels = _mapper.Map<List<PlayerRankingViewModel>>(playerRankings);
            return playerRankingViewModels;
        }

        // GET: api/PlayerRankings/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlayerRankingViewModel>> GetPlayerRanking(int id)
        {
            var playerRanking = await _context.PlayerRankings
                .FirstOrDefaultAsync(x=>x.PlayerId==id);

            if (playerRanking == null)
            {
                return NotFound();
            }

            var playerRankingViewModel = _mapper.Map<PlayerRankingViewModel>(playerRanking);
            return playerRankingViewModel;
        }

        // // PUT: api/PlayerRankings/5
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutPlayerRanking(int id, PlayerRanking playerRanking)
        // {
        //     if (id != playerRanking.Id)
        //     {
        //         return BadRequest();
        //     }
        //
        //     _context.Entry(playerRanking).State = EntityState.Modified;
        //
        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!PlayerRankingExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }
        //
        //     return NoContent();
        // }
        //
        // // POST: api/PlayerRankings
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<PlayerRanking>> PostPlayerRanking(PlayerRanking playerRanking)
        // {
        //     _context.PlayerRankings.Add(playerRanking);
        //     await _context.SaveChangesAsync();
        //
        //     return CreatedAtAction("GetPlayerRanking", new {id = playerRanking.Id}, playerRanking);
        // }
        //
        // // DELETE: api/PlayerRankings/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeletePlayerRanking(int id)
        // {
        //     var playerRanking = await _context.PlayerRankings.FindAsync(id);
        //     if (playerRanking == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     _context.PlayerRankings.Remove(playerRanking);
        //     await _context.SaveChangesAsync();
        //
        //     return NoContent();
        // }

        private bool PlayerRankingExists(int id)
        {
            return _context.PlayerRankings.Any(e => e.Id == id);
        }
    }
}