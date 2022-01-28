using System;
using System.Collections.Generic;

namespace PRA_WebAPI.Entities
{
    public partial class Game
    {
        public Game()
        {
            Players = new HashSet<Player>();
        }

        public int Id { get; set; }
        public int QuizId { get; set; }
        public string GamePin { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }

        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<Player> Players { get; set; }
    }
}
