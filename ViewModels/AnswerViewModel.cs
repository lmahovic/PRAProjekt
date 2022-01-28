namespace PRA_WebAPI.ViewModels;

public class AnswerViewModel
{
    public int Id { get; set; }
    public string AnswerText { get; set; }
    public bool Correct { get; set; }
    public byte AnswerOrder { get; set; }
}