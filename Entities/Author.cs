using System;
using System.Collections.Generic;

namespace PRA_WebAPI.Entities
{
    public partial class Author
    {
        public Author()
        {
            Quizzes = new HashSet<Quiz>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}
