using GraphQL.GraphExceptions;
using GraphQL.Mutations;
using GraphQL.Queries;
using GraphQL.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddSingleton<AuthorRepository>();
services.AddSingleton<BookRepository>();

services.AddAutoMapper(typeof(GraphQLProfile));

var graphQLBuilder = services.AddGraphQLServer();

graphQLBuilder
    .AddMutationConventions(applyToAllMutations: true)
    .AddErrorInterfaceType<IRootError>()
    .AddMutationType<BookMutation>();

graphQLBuilder.AddQueryType<RootQuery>()
   .AddTypeExtension<AuthorQueries>()
   .AddTypeExtension<AuthorBooksQueries>()
   .AddTypeExtension<BookQueries>()
   .AddTypeExtension<BookAuthorQueries>();

var app = builder.Build();


app.MapGraphQL();

app.Run();
