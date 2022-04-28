using Microsoft.AspNetCore.Authentication;

namespace GraphQL.Configuration.ApiKey
{
    public class ApiKeyAuthNOptions : AuthenticationSchemeOptions
    {
        public string ApiKey { get; set; }
        public string QueryStringKey { get; set; }
    }
}
