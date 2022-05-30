namespace GraphQL.Extensions.Authentication
{
    public record AppSettings(string Secret, string Emissor, string ValidoEm, int ExpiracaoHoras);
}
