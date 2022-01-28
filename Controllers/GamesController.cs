using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRA_WebAPI.Context;
using PRA_WebAPI.ViewModels;

namespace PRA_WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
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
            return NotFound();
        }

        return Ok(_mapper.Map<GameViewModel>(game));
    }


}