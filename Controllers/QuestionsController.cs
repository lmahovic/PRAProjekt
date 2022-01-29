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
        public async Task<ActionResult<List<QuestionViewModel>>> GetQuestion(int quizId)
        {
            try
            {
                var quiz = await _context.Quizzes
                    .Include(q => q.Questions)
                    .ThenInclude(question => question.Answers)
                    .SingleAsync(q => q.Id == quizId);

                var questionViewModels = _mapper.Map<List<QuestionViewModel>>(quiz.Questions);
                
                questionViewModels.Sort((x,y)=> x.QuestionOrder.CompareTo(y.QuestionOrder));
                
                foreach (var questionViewModel in questionViewModels)
                {
                    questionViewModel.Answers.Sort((x,y)=>x.AnswerOrder.CompareTo(y.AnswerOrder));
                }
                
                return Ok(questionViewModels);
            }
            catch (Exception)
            {
                return NotFound("Quiz not found!");
            }

        }


    }

}