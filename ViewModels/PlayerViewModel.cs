using System.ComponentModel.DataAnnotations;

namespace PRA_WebAPI.ViewModels;

public class PlayerViewModel
{
    public int Id { get; set; }
    [Required]
    public string Nickname { get; set; }
    [Range(1,int.MaxValue,ErrorMessage = "GameId is not valid!")]
    public int GameId { get; set; }
    
}