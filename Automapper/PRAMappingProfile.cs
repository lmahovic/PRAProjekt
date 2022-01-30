using AutoMapper;
using PRA_WebAPI.Entities;
using PRA_WebAPI.ViewModels;

namespace PRA_WebAPI.Automapper;

public class PraMappingProfile : Profile
{
    public PraMappingProfile()
    {
        CreateMap<Game, GameViewModel>();
        CreateMap<Player, PlayerViewModel>()
            .ReverseMap();
        CreateMap<Question, QuestionViewModel>()
            .ReverseMap();
        CreateMap<Answer, AnswerViewModel>()
            .ReverseMap();
        CreateMap<PlayerQuestionAnswer, PlayerQuestionAnswerViewModel>()
            .ReverseMap();
        CreateMap<PlayerRanking, PlayerRankingViewModel>()
            .ReverseMap();
    }
}