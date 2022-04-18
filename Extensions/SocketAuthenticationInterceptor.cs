using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Messages;
using Microsoft.Extensions.Options;

namespace GraphQL.Extensions
{
    public class SocketAuthenticationInterceptor : DefaultSocketSessionInterceptor
    {
        private readonly AppSettings appSettings;

        public SocketAuthenticationInterceptor(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override ValueTask<ConnectionStatus> OnConnectAsync(ISocketConnection connection, InitializeConnectionMessage message, CancellationToken cancellationToken)
        {
            if (message.Payload == null || !message.Payload.Any() || message.Payload["Authorization"] is not string auth)
            {
                return ValueTask.FromResult(ConnectionStatus.Reject("No Authorization"));
            }

            var token = auth.Replace("Bearer", "").Trim();
            try
            {
                new JwtAuthenticatorCommand(appSettings, connection.HttpContext).Execute(token);
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
