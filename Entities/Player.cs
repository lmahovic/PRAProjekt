using System;
using System.Collections.Generic;

namespace PRA_WebAPI.Entities
{
    public partial class Player
    {
        public Player()
        {
            PlayerQuestionAnswers = new HashSet<PlayerQuestionAnswer>();
        }

        public int Id { get; set; }
        public string Nickname { get; set; }
        public int GameId { get; set; }
        public bool HasQuit { get; set; }

        public virtual Game Game { get; set; }
        public virtual PlayerRanking PlayerRanking { get; set; }
        public virtual ICollection<PlayerQuestionAnswer> PlayerQuestionAnswers { get; set; }
    }
}
