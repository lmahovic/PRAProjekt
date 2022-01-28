using System;
using System.Collections.Generic;

namespace PRA_WebAPI.Entities
{
    public partial class PlayerQuestionAnswer
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public double? AnswerTime { get; set; }
        public int? Points { get; set; }

        public virtual Answer Answer { get; set; }
        public virtual Player Player { get; set; }
        public virtual Question Question { get; set; }
    }
}
