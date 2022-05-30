using AutoMapper;
using GraphQL.Mutations;
using GraphQL.Operations.Mutations.Dtos;

class GraphQLProfile : Profile
{
    public GraphQLProfile()
    {
        CreateMap<BookDto, Book>()
            .ForMember(c => c.Id, o => o.MapFrom((s, d) => s.Id == Guid.Empty ? Guid.NewGuid() : s.Id))
            .ReverseMap()
            .ForMember(c => c.Id, o => o.MapFrom((s, d) => d.Id == Guid.Empty ? Guid.NewGuid() : d.Id));
    }
}
