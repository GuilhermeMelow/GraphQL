import { createApolloProvider } from '@vue/apollo-option'
import { split } from '@apollo/client/core'
import { getMainDefinition } from 'apollo-utilities'
import { WebSocketLink } from '@apollo/client/link/ws'
import { ApolloClient, InMemoryCache, createHttpLink } from '@apollo/client/core'

const cache = new InMemoryCache()

const httpLink = createHttpLink({
    uri: 'http://localhost:5000/graphql'
})

const wsLink = new WebSocketLink({
    uri: 'ws://localhost:5000/graphql',
    options: {
        reconnect: true
    }
})

const splittedLink = split(
    ({ query }) => {
        const definition = getMainDefinition(query)

        return (
            definition.kind === 'OperationDefinition' &&
            definition.operation === 'subscription'
        )
    }, wsLink, httpLink)

export const apolloClient = new ApolloClient({
    link: splittedLink,
    cache
})

export const apolloProvider = createApolloProvider({
    defaultClient: apolloClient
})
