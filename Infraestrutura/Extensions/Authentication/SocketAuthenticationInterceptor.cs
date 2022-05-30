using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Messages;

namespace GraphQL.Extensions.Authentication
{
    public class SocketAuthenticationInterceptor : DefaultSocketSessionInterceptor
    {
        private readonly AuthenticationProviderFactory authenticationProviderFactory;

        public SocketAuthenticationInterceptor(AuthenticationProviderFactory authenticationProviderFactory)
        {
            this.authenticationProviderFactory = authenticationProviderFactory;
        }

        public override ValueTask<ConnectionStatus> OnConnectAsync(ISocketConnection connection, InitializeConnectionMessage message, CancellationToken cancellationToken)
        {
            if (message.Payload == null || !message.Payload.Any() || message.Payload["Authorization"] is not string authToken)
            {
                return ValueTask.FromResult(ConnectionStatus.Reject("No Authorization"));
            }

            try
            {
                var provider = authenticationProviderFactory.GetProvider(authToken);
                provider.Authenticate(authToken);
            }
            catch (Exception ex)
            {
                return ValueTask.FromResult(ConnectionStatus.Reject(ex.Message));
            }

            Task.Run(() => RefreshConnectionAsync(connection, cancellationToken), cancellationToken);

            return base.OnConnectAsync(connection, message, cancellationToken);
        }

        private static async Task RefreshConnectionAsync(ISocketConnection connection, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);

            await connection.CloseAsync("Refresh", SocketCloseStatus.NormalClosure, cancellationToken);
        }
    }
}
