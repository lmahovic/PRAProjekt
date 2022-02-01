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

namespace PRA_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly PRAQuizContext _context;
        private readonly IMapper _mapper;

        public QuizzesController(PRAQuizContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // // GET: api/Quizzes
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzes()
        // {
        //     return await _context.Quizzes.ToListAsync();
        // }

        // GET: api/Quizzes/5
        [HttpGet("{authorId:int}")]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuiz(int authorId)
        {
            var quizzes = await _context.Quizzes
                .Where(x=>x.AuthorId==authorId)
                .ToListAsync();

            if (quizzes.Count==0)
            {
                return NotFound("No quizzes found for the specified author!");
            }

            return Ok(_mapper.Map<List<QuizViewModel>>(quizzes));
        }

        // // PUT: api/Quizs/5
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutQuiz(int id, Quiz quiz)
        // {
        //     if (id != quiz.Id)
        //     {
        //         return BadRequest();
        //     }
        //
        //     _context.Entry(quiz).State = EntityState.Modified;
        //
        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!QuizExists(id))
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
        // // POST: api/Quizs
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<Quiz>> PostQuiz(Quiz quiz)
        // {
        //     _context.Quizzes.Add(quiz);
        //     await _context.SaveChangesAsync();
        //
        //     return CreatedAtAction("GetQuiz", new { id = quiz.Id }, quiz);
        // }
        //
        // // DELETE: api/Quizs/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteQuiz(int id)
        // {
        //     var quiz = await _context.Quizzes.FindAsync(id);
        //     if (quiz == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     _context.Quizzes.Remove(quiz);
        //     await _context.SaveChangesAsync();
        //
        //     return NoContent();
        // }

        private bool QuizExists(int id)
        {
            return _context.Quizzes.Any(e => e.Id == id);
        }
    }
}