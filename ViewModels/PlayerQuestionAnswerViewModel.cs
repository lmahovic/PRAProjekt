namespace PRA_WebAPI.ViewModels;

public class PlayerQuestionAnswerViewModel
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int QuestionId { get; set; }
    public int? AnswerId { get; set; }
    public long? AnswerTime { get; set; }
    public int? Points { get; set; }
}