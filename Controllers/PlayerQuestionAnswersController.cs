using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRA_WebAPI.Context;
using PRA_WebAPI.Entities;

namespace PRA_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerQuestionAnswersController : ControllerBase
    {
        private readonly PRAQuizContext _context;

        public PlayerQuestionAnswersController(PRAQuizContext context)
        {
            _context = context;
        }

        // // GET: api/PlayerQuestionAnswers
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<PlayerQuestionAnswer>>> GetPlayerQuestionAnswers()
        // {
        //     return await _context.PlayerQuestionAnswers.ToListAsync();
        // }

        // GET: api/PlayerQuestionAnswers/5
        [HttpGet]
        public async Task<ActionResult<int>> GetPlayerQuestionAnswer(int playerId, int questionId)
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
        public async Task<IActionResult> PutPlayerQuestionAnswer(int id, int answerId, double answerTime)
        {
            var playerQuestionAnswer = await _context.PlayerQuestionAnswers
                .FindAsync(id);

            if (playerQuestionAnswer == null)
            {
                return BadRequest("Player question not found");
            }

            playerQuestionAnswer.AnswerId = answerId == 0 ? null : answerId;
            playerQuestionAnswer.AnswerTime = answerTime;

            if (playerQuestionAnswer.AnswerId == null)
            {
                playerQuestionAnswer.Points = 0;
            }
            else
            {
                var answer = await _context.Answers
                    .FindAsync(answerId);

                if (answer == null)
                {
                    return BadRequest("Answer not found");
                }

                if (answer.Correct)
                {
                    var question = await _context.Questions
                        .FindAsync(answer.QuestionId);

                    playerQuestionAnswer.Points = CalculatePoints(question, answerTime);
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
                if (!PlayerQuestionAnswerExists(id))
                {
                    return NotFound();
                }
            }

            return Ok();
        }
        private static int CalculatePoints(Question question, double answerTime)
        {
            return (int) (100 + 100 * (1 - answerTime / question.TimeLimit));
        }

        // // POST: api/PlayerQuestionAnswers
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<PlayerQuestionAnswer>> PostPlayerQuestionAnswer(PlayerQuestionAnswer playerQuestionAnswer)
        // {
        //     _context.PlayerQuestionAnswers.Add(playerQuestionAnswer);
        //     await _context.SaveChangesAsync();
        //
        //     return CreatedAtAction("GetPlayerQuestionAnswer", new {id = playerQuestionAnswer.Id}, playerQuestionAnswer);
        // }
        //
        // // DELETE: api/PlayerQuestionAnswers/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeletePlayerQuestionAnswer(int id)
        // {
        //     var playerQuestionAnswer = await _context.PlayerQuestionAnswers.FindAsync(id);
        //     if (playerQuestionAnswer == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     _context.PlayerQuestionAnswers.Remove(playerQuestionAnswer);
        //     await _context.SaveChangesAsync();
        //
        //     return NoContent();
        // }
        //
        private bool PlayerQuestionAnswerExists(int id)
        {
            return _context.PlayerQuestionAnswers.Any(e => e.Id == id);
        }
    }
}