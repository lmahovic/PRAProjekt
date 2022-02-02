using System;
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
        public async Task<ActionResult<IEnumerable<PlayerRankingViewModel>>> GetPlayerRankings([FromQuery] int gameId)
        {
            var playerRankings = await _context.PlayerRankings
                .Include(x => x.Player)
                .Where(x => x.Player.GameId == gameId)
                .ToListAsync();

            var playerRankingViewModels = _mapper.Map<List<PlayerRankingViewModel>>(playerRankings);
            return playerRankingViewModels;
        }

        // GET: api/PlayerRankings/5
        [HttpGet("{playerId:int}")]
        public async Task<ActionResult<PlayerRankingViewModel>> GetPlayerRanking(int playerId)
        {
            var playerRanking = await _context.PlayerRankings
                .FirstOrDefaultAsync(x => x.PlayerId == playerId);

            if (playerRanking == null)
            {
                return NotFound();
            }

            var playerRankingViewModel = _mapper.Map<PlayerRankingViewModel>(playerRanking);
            return playerRankingViewModel;
        }

        // PUT: api/PlayerRankings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{gameId:int}")]
        public async Task<IActionResult> PutPlayerRankings(int gameId)
        {
            var playerTotalScoreDictionary = await CreatePlayerDictionary(gameId);

            if (playerTotalScoreDictionary.Count == 0)
            {
                return NotFound($"No records were found for gameId {gameId}!");
            }

            var lastRanking = 1;
            var playerTotalScoreDictionaryKeys = playerTotalScoreDictionary.Keys.ToList();
            var playerTotalScoreDictionaryValues = playerTotalScoreDictionary.Values.ToList();

            var playerRankingList = await _context.PlayerRankings
                .Where(x => playerTotalScoreDictionaryKeys.Contains(x.PlayerId))
                .ToListAsync();


            for (var i = 0; i < playerTotalScoreDictionary.Count; i++)
            {
                var oldPlayerRanking = playerRankingList
                    .Single(x => x.PlayerId == playerTotalScoreDictionaryKeys[i]);

                oldPlayerRanking.TotalPoints = playerTotalScoreDictionaryValues[i];

                if (i == 0)
                {
                    oldPlayerRanking.Ranking = 1;
                }
                else
                {
                    if (playerTotalScoreDictionaryValues[i] == playerTotalScoreDictionaryValues[i - 1])
                    {
                        oldPlayerRanking.Ranking = lastRanking;
                    }
                    else
                    {
                        oldPlayerRanking.Ranking = i + 1;
                        lastRanking++;
                    }
                }
            }
            await _context.SaveChangesAsync();

            return Ok("Rank list updated!");
        }
        //
        // POST: api/PlayerRankings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{gameId:int}")]
        public async Task<ActionResult<IEnumerable<PlayerRankingViewModel>>> PostPlayerRanking([FromRoute] int gameId)
        {
            var playerTotalScoreDictionary = await CreatePlayerDictionary(gameId);

            if (playerTotalScoreDictionary.Count == 0)
            {
                return NotFound($"No records were found for gameId {gameId}!");
            }

            var lastRanking = 1;
            var playerRankingList = new List<PlayerRanking>();
            var playerTotalScoreDictionaryKeys = playerTotalScoreDictionary.Keys.ToList();
            var playerTotalScoreDictionaryValues = playerTotalScoreDictionary.Values.ToList();

            for (var i = 0; i < playerTotalScoreDictionary.Count; i++)
            {
                var newPlayerRanking = new PlayerRanking
                {
                    PlayerId = playerTotalScoreDictionaryKeys[i],
                    TotalPoints = playerTotalScoreDictionaryValues[i]
                };

                if (playerRankingList.Count == 0)
                {
                    newPlayerRanking.Ranking = 1;
                }
                else
                {
                    if (playerTotalScoreDictionaryValues[i] == playerTotalScoreDictionaryValues[i - 1])
                    {
                        newPlayerRanking.Ranking = lastRanking;
                    }
                    else
                    {
                        newPlayerRanking.Ranking = i + 1;
                        lastRanking++;
                    }
                }

                playerRankingList.Add(newPlayerRanking);
            }

            _context.PlayerRankings.AddRange(playerRankingList);
            await _context.SaveChangesAsync();

            var playerRankingViewModels = _mapper.Map<IEnumerable<PlayerRankingViewModel>>(playerRankingList);

            return CreatedAtAction(nameof(GetPlayerRankings), new {gameId}, playerRankingViewModels);
        }
        private async Task<Dictionary<int, int>> CreatePlayerDictionary(int gameId)
        {

            return await _context.PlayerQuestionAnswers
                .Include(x => x.Player)
                .Where(x => x.Player.GameId == gameId)
                .GroupBy(x => x.PlayerId)
                .Select(x =>
                    new
                    {
                        playerId = x.Key,
                        playerTotalScore =
                            x.Sum(y => y.Points ?? 0)
                    })
                .OrderByDescending(x => x.playerTotalScore)
                .ToDictionaryAsync(
                    x => x.playerId,
                    x => x.playerTotalScore);
        }

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