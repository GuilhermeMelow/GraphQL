using AutoMapper;
using GraphQL.Models;
using GraphQL.Mutations;

class GraphQLProfile : Profile
{
    public GraphQLProfile()
    {
        CreateMap<BookDto, Book>()
            .ForMember(c => c.Id, o => o.MapFrom((s, d) => s.Id ?? Guid.NewGuid()))
            .ReverseMap()
            .ForMember(c => c.Id, o => o.MapFrom((s, d) => d.Id ?? Guid.NewGuid()));
    }
}
