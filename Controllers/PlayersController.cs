using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRA_WebAPI.Context;
using PRA_WebAPI.Entities;
using PRA_WebAPI.ViewModels;

namespace PRA_WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly PRAQuizContext _context;
    private readonly IMapper _mapper;

    public PlayersController(PRAQuizContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Players?gameId=x
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerViewModel>>> GetPlayersByQuiz([FromQuery] int gameId)
    {
        if (!_context.Games.Any(x => x.Id == gameId))
        {
            return BadRequest($"Game {gameId} does not exist!");
        }
        return Ok(_mapper.Map<IEnumerable<PlayerViewModel>>(await _context.Players
            .Where(x => x.GameId == gameId)
            .ToListAsync()));
    }

    // GET: api/Players/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlayerViewModel>> GetPlayer(int id) =>
        Ok(_mapper.Map<PlayerViewModel>(await _context.Players.FindAsync(id)));

    // POST: api/Players
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<PlayerViewModel>> PostPlayer([FromBody] PlayerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingPlayersNicknames = await _context.Players
            .Where(player => player.GameId == model.GameId)
            .Select(player => player.Nickname.ToUpper())
            .ToListAsync();

        // Production CODE
        // **************

        if (existingPlayersNicknames.Contains(model.Nickname.ToUpper()))
        {
            return BadRequest("Player with the selected nickname already exists!");
        }

        // //@Todo test - makni 
        // //Test CODE
        // //**************
        //
        // if (existingPlayersNicknames.Contains(model.Nickname.ToUpper()))
        // {
        //     var existing = await _context.Players.SingleAsync(x => x.Nickname == model.Nickname.ToUpper());
        //     return CreatedAtAction("GetPlayer", new {id = existing.Id}, _mapper.Map<PlayerViewModel>(existing));
        // }


        var player = _mapper.Map<Player>(model);
        try
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPlayer", new {id = player.Id}, _mapper.Map<PlayerViewModel>(player));
        }
        catch (Exception)
        {
            return BadRequest("Error writing to database");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutPlayer(int id)
    {
        var player = await _context.Players
            .FindAsync(id);

        if (player == null)
        {
            return BadRequest("Player not found");
        }

        player.HasQuit = true;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PlayerExists(id))
            {
                return NotFound();
            }
        }

        return Ok();
    }


    private bool PlayerExists(int id)
    {
        return _context.Players.Any(e => e.Id == id);
    }


}