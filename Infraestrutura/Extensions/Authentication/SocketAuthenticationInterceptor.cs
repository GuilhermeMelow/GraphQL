using GraphQL.Extensions.Authentication.Providers;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.Extensions.Authentication
{
    public class SocketAuthenticationInterceptor : DefaultSocketSessionInterceptor
    {
        private readonly IServiceProvider serviceProvider;

        public SocketAuthenticationInterceptor(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public override ValueTask<ConnectionStatus> OnConnectAsync(ISocketConnection connection, InitializeConnectionMessage message, CancellationToken cancellationToken)
        {
            if (message.Payload == null || !message.Payload.Any() || message.Payload["Authorization"] is not string authToken)
            {
                return ValueTask.FromResult(ConnectionStatus.Reject("No Authorization"));
            }

            using var scope = serviceProvider.CreateScope();
            var authFactory = scope.ServiceProvider.GetRequiredService<AuthenticationProviderFactory>();
            var provider = authFactory.GetProvider(authToken);

            try
            {
                provider.Authenticate(authToken);
            }
            catch (AuthenticationException ex)
            {
                return ValueTask.FromResult(ConnectionStatus.Reject(ex.Message));
            }

            Task.Run(() => CloseConnection(connection, cancellationToken), cancellationToken);

            return base.OnConnectAsync(connection, message, cancellationToken);
        }

        private static async Task CloseConnection(ISocketConnection connection, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);

            await connection.CloseAsync(message: "Timeout", SocketCloseStatus.NormalClosure, cancellationToken);
        }
    }
}
