﻿using GraphQL.Extensions.Authentication;
using GraphQL.GraphExceptions;
using GraphQL.Operations.Mutations;
using GraphQL.Operations.Queries;
using GraphQL.Operations.Subscriptions;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.Configuration
{
    public static class GraphQLConfiguration
    {
        public static IServiceCollection AddGraphql(this IServiceCollection services)
        {
            var graphQLBuilder = services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddSocketSessionInterceptor<SocketAuthenticationInterceptor>()
                .AddInMemorySubscriptions();

            graphQLBuilder
                .AddSubscriptionType<Subscription>();

            graphQLBuilder
                .AddMutationConventions(applyToAllMutations: true)
                .AddErrorInterfaceType<IRootError>()
                .AddMutationType<Mutation>()
                .AddTypeExtension<LoginMutation>()
                .AddTypeExtension<BookMutation>();

            graphQLBuilder.AddQueryType<RootQuery>()
               .AddTypeExtension<AuthorQueries>()
               .AddTypeExtension<AuthorBooksQueries>()
               .AddTypeExtension<BookQueries>()
               .AddTypeExtension<BookAuthorQueries>();

            return services;
        }

        public static void UsePlayground(this WebApplication app)
        {
            app.UsePlayground(new PlaygroundOptions
            {
                SubscriptionPath = "/graphql"
            });

            app.MapGraphQL();
        }
    }
}