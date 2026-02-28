using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.SqlServer;

public class PigeonSqlServerOptionsValidator : IValidateOptions<PigeonSqlServerOptions>
{
    public ValidateOptionsResult Validate(string? name, PigeonSqlServerOptions options)
    {
        Guard.AgainstNull(options);

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return ValidateOptionsResult.Fail(Resources.ConnectionStringOptionException);
        }

        return ValidateOptionsResult.Success;
    }
}