using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GraphQL.Extensions
{
    public class HttpAuthenticationInterceptor : DefaultHttpRequestInterceptor
    {
        private readonly AppSettings appSettings;
        public HttpAuthenticationInterceptor(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public override ValueTask OnCreateAsync(HttpContext context, IRequestExecutor requestExecutor, IQueryRequestBuilder requestBuilder, CancellationToken cancellationToken)
        {
            var auth = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(auth))
            {
                var token = auth.Replace("Bearer", "").Trim();
                try
                {
                    new JwtAuthenticatorCommand(appSettings, context).Execute(token);
                }
                catch (Exception ex)
                {
                    return ValueTask.FromException(ex);
                }
            }

            return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
        }

    }
}
