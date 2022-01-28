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
    public class QuestionsController : ControllerBase
    {
        private readonly PRAQuizContext _context;
        private readonly IMapper _mapper;

        public QuestionsController(PRAQuizContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        // GET: api/Questions/5
        [HttpGet("{quizId:int}")]
        public async Task<ActionResult<IEnumerable<QuestionViewModel>>> GetQuestion(int quizId)
        {
            Quiz quiz;
            try
            {
                quiz = await _context.Quizzes
                    .Include(q => q.Questions)
                    .ThenInclude(question => question.Answers)
                    .SingleAsync(q => q.Id == quizId);
            }
            catch (Exception)
            {
                return NotFound("Quiz not found!");
            }

            return Ok(_mapper.Map<IEnumerable<QuestionViewModel>>(quiz.Questions));
        }


        
    }

}