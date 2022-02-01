using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class PlayerQuestionAnswersController : ControllerBase
    {
        private readonly PRAQuizContext _context;
        private readonly IMapper _mapper;

        public PlayerQuestionAnswersController(PRAQuizContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerQuestionAnswerViewModel>>> GetPlayerQuestionAnswersByQuestion(
            [FromQuery] int gameId, [FromQuery] int questionId)
        {
            var playerQuestionAnswers = await _context.PlayerQuestionAnswers
                .Include(x => x.Player)
                .Where(x =>
                    x.Player.GameId == gameId &&
                    x.QuestionId == questionId)
                .ToListAsync();

            return Ok(_mapper.Map<List<PlayerQuestionAnswerViewModel>>(playerQuestionAnswers));
        }

        //Check if all players have answered
        // GET: api/PlayerQuestionAnswers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<bool>> GetPlayerQuestionAnswers([FromRoute] int id)
        {
            if (!PlayerQuestionAnswerExists(id))
            {
                return NotFound($"PlayerQuestionAnswer with the id {id} was not found!");
            }

            var playerQuestionAnswer = await _context.PlayerQuestionAnswers
                .SingleAsync(x => x.Id == id);

            await _context.Players.Where(x => x.Id == playerQuestionAnswer.PlayerId).LoadAsync();

            return Ok(!await _context.PlayerQuestionAnswers
                .Include(x => x.Player)
                .AnyAsync(x =>
                    x.QuestionId == playerQuestionAnswer.QuestionId &&
                    x.Player.GameId == playerQuestionAnswer.Player.GameId &&
                    x.Player.HasQuit == false &&
                    x.Points == null));

        }

        // GET: api/PlayerQuestionAnswers/1/0
        [HttpGet("{playerId:int}/{questionId:int}")]
        public async Task<ActionResult<int>> GetPlayerQuestionAnswer([FromRoute] int playerId, [FromRoute] int questionId)
        {
            var playerQuestionAnswer = await _context.PlayerQuestionAnswers
                .FirstOrDefaultAsync(answer =>
                    answer.PlayerId == playerId &&
                    answer.QuestionId == questionId);

            return Ok(playerQuestionAnswer?.Id ?? 0);
        }

        // PUT: api/PlayerQuestionAnswers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ActionResult<int>> PutPlayerQuestionAnswer(PlayerQuestionAnswerViewModel playerQuestionAnswerViewModel)
        {

            if (!PlayerQuestionAnswerExists(playerQuestionAnswerViewModel.Id))
            {
                return BadRequest("Player question not found");
            }

            var playerQuestionAnswer = await _context.PlayerQuestionAnswers
                .SingleAsync(x => x.Id == playerQuestionAnswerViewModel.Id);

            _mapper.Map(playerQuestionAnswerViewModel, playerQuestionAnswer);

            if (playerQuestionAnswer.AnswerId == 0)
            {
                playerQuestionAnswer.Points = 0;
                playerQuestionAnswer.AnswerId = null;
            }
            else
            {
                var answer = await _context.Answers
                    .FindAsync(playerQuestionAnswer.AnswerId);

                if (answer == null)
                {
                    return BadRequest("Answer not found");
                }

                if (answer.Correct)
                {
                    var question = await _context.Questions
                        .FindAsync(answer.QuestionId);

                    playerQuestionAnswer.Points = CalculatePoints(question, playerQuestionAnswer.AnswerTime!.Value);
                }
                else
                {
                    playerQuestionAnswer.Points = 0;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerQuestionAnswerExists(playerQuestionAnswer.Id))
                {
                    return NotFound();
                }
            }

            return Ok(playerQuestionAnswer.Points);
        }
        private static int CalculatePoints(Question question, long answerTime)
        {
            var answerTimeDouble = (double) answerTime;
            return (int) Math.Round(100 + 100 * (1 - answerTimeDouble / (question.TimeLimit * 1000)));
        }

        // POST: api/PlayerQuestionAnswers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlayerQuestionAnswerViewModel>> PostPlayerQuestionAnswers(
            [FromBody] IEnumerable<PlayerQuestionAnswerViewModel> playerQuestionAnswers)
        {
            try
            {
                var gameId = await _context.Players
                    .Where(pqa => playerQuestionAnswers
                        .Select(p => p.PlayerId)
                        .Contains(pqa.Id))
                    .Select(x => x.GameId)
                    .Distinct()
                    .SingleAsync();

                var questionId = playerQuestionAnswers.Select(x => x.QuestionId)
                    .Distinct().Single();

                var playerQuestionAnswerEntities = _mapper.Map<List<PlayerQuestionAnswer>>(playerQuestionAnswers);
                
                foreach (var playerQuestionAnswerEntity in playerQuestionAnswerEntities)
                {
                    
                }

                var existingPqa = await _context.PlayerQuestionAnswers.ToListAsync();
                foreach (var playerQuestionAnswerEntity in playerQuestionAnswerEntities)
                {
                    playerQuestionAnswerEntity.Id = 0;
                    if (existingPqa.Any(x =>
                            x.PlayerId == playerQuestionAnswerEntity.PlayerId &&
                            x.QuestionId == playerQuestionAnswerEntity.QuestionId))
                    {
                        return BadRequest(
                            $"Player {playerQuestionAnswerEntity.PlayerId} already has a " +
                            $"record for question {playerQuestionAnswerEntity.QuestionId}");
                    }
                }
                _context.PlayerQuestionAnswers.AddRange(playerQuestionAnswerEntities);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPlayerQuestionAnswersByQuestion),
                    new {gameId, questionId},
                    _mapper.Map<List<PlayerQuestionAnswerViewModel>>(playerQuestionAnswerEntities));
            }
            catch (Exception)
            {
                return BadRequest("Game id and question ids are not unique!");
            }

        }

        // DELETE: api/PlayerQuestionAnswers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerQuestionAnswer(int id)
        {
            var playerQuestionAnswer = await _context.PlayerQuestionAnswers.FindAsync(id);
            if (playerQuestionAnswer == null)
            {
                return NotFound();
            }

            _context.PlayerQuestionAnswers.Remove(playerQuestionAnswer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerQuestionAnswerExists(int id)
        {
            return _context.PlayerQuestionAnswers.Any(e => e.Id == id);
        }
    }
}