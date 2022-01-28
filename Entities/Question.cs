using System;
using System.Collections.Generic;

namespace PRA_WebAPI.Entities
{
    public partial class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
            PlayerQuestionAnswers = new HashSet<PlayerQuestionAnswer>();
        }

        public int Id { get; set; }
        public string QuestionText { get; set; }
        public int TimeLimit { get; set; }
        public int QuizId { get; set; }
        public byte QuestionOrder { get; set; }

        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<PlayerQuestionAnswer> PlayerQuestionAnswers { get; set; }
    }
}
