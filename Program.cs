using GraphQL.GraphExceptions;
using GraphQL.Mutations;
using GraphQL.Queries;
using GraphQL.Repositories;
using GraphQL.Subscriptions;
using GraphQL.UseCases;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddSingleton<AuthorRepository>();
services.AddSingleton<BookRepository>();
services.AddSingleton<IBookAddUseCase, BookAddUseCase>();

services.AddAutoMapper(typeof(GraphQLProfile));

services.AddInMemorySubscriptions();

services.AddCors(c => c.AddDefaultPolicy(builder =>
{
    builder.WithOrigins("http://localhost:8080")
        .AllowAnyHeader()
        .AllowCredentials();
}));

var graphQLBuilder = services.AddGraphQLServer();

graphQLBuilder
    .AddSubscriptionType<Subscription>();

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

app.UseRouting();

app.UseWebSockets();

app.UsePlayground(new PlaygroundOptions
{
    SubscriptionPath = "/graphql"
});

app.MapGraphQL();

app.UseCors();

app.Run();
