using System;

namespace PRA_WebAPI.ViewModels;

public class GameViewModel
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string GamePin { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? FinishTime { get; set; }
}