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
    public class AuthorsController : ControllerBase
    {
        private readonly PRAQuizContext _context;
        private readonly IMapper _mapper;

        public AuthorsController(PRAQuizContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // // GET: api/Authors
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        // {
        //     return await _context.Authors.ToListAsync();
        // }

        // GET: api/Authors/5
        [HttpGet("{email}")]
        public async Task<ActionResult<AuthorViewModel>> GetAuthor(string email)
        {
            try
            {
                var author = await _context.Authors
                    .SingleAsync(x => x.Email == email);

                return _mapper.Map<AuthorViewModel>(author);
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<bool>> GetAuthor([FromRoute] int id, [FromQuery] string password)
        {
            try
            {
                var author = await _context.Authors
                    .SingleAsync(x => x.Id == id);

                return author.Password == password;
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        // // PUT: api/Authors/5
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutAuthor(int id, Author author)
        // {
        //     if (id != author.Id)
        //     {
        //         return BadRequest();
        //     }
        //
        //     _context.Entry(author).State = EntityState.Modified;
        //
        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!AuthorExists(id))
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
        // // POST: api/Authors
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<Author>> PostAuthor(Author author)
        // {
        //     _context.Authors.Add(author);
        //     await _context.SaveChangesAsync();
        //
        //     return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        // }
        //
        // // DELETE: api/Authors/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteAuthor(int id)
        // {
        //     var author = await _context.Authors.FindAsync(id);
        //     if (author == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     _context.Authors.Remove(author);
        //     await _context.SaveChangesAsync();
        //
        //     return NoContent();
        // }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}