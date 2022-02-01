using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRA_WebAPI.Context;
using PRA_WebAPI.Entities;
using PRA_WebAPI.ViewModels;

namespace PRA_WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private const int PinLength = 5;

    private readonly PRAQuizContext _context;
    private readonly IMapper _mapper;

    public GamesController(PRAQuizContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Games/5
    [HttpGet("{gamePin}")]
    public async Task<ActionResult<GameViewModel>> GetGame(string gamePin)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(game =>
                game.GamePin == gamePin &&
                game.StartTime == null);

        if (game == null)
        {
            return BadRequest("Game pin not found!");
        }

        return Ok(_mapper.Map<GameViewModel>(game));
    }

    // POST: api/Games/quizId
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("{quizId:int}")]
    public async Task<ActionResult<GameViewModel>> PostGame([FromRoute] int quizId)
    {
        if (!_context.Quizzes.Any(e => e.Id == quizId))
        {
            return BadRequest("Quiz does not exist!");
        }

        var existingPins = await _context.Games
            .Where(x => x.StartTime == null)
            .Select(x => x.GamePin)
            .ToListAsync();

        string newPin;
        do
        {
            newPin = GetNewRandomPin();
        } while (existingPins.Contains(newPin));

        var newGame = new Game
        {
            QuizId = quizId,
            GamePin = newPin
        };

        _context.Games.Add(newGame);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGame), new {gamePin = newGame.GamePin}, _mapper.Map<GameViewModel>(newGame));
    }
    
    [HttpPut]
    public async Task<IActionResult> PutFinishTime([FromQuery]int id)
    {
        var game = await _context.Games
            .FindAsync(id);

        if (game == null)
        {
            return BadRequest($"Game with id {id} not found");
        }

        game.FinishTime = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok("Finish time set");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutStartTime(int id)
    {
        var game = await _context.Games
            .FindAsync(id);

        if (game == null)
        {
            return BadRequest($"Game with id {id} not found");
        }

        game.StartTime = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok("Start time set");
    }

    private static string GetNewRandomPin()
    {
        StringBuilder stringBuilder = new();
        var random = new Random();
        for (var i = 0; i < PinLength; i++)
        {
            stringBuilder.Append(random.Next(0, 9));
        }

        return stringBuilder.ToString();
    }


}