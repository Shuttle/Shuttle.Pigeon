using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Data;

public class PigeonDataOptionsValidator : IValidateOptions<PigeonDataOptions>
{
    public ValidateOptionsResult Validate(string? name, PigeonDataOptions options)
    {
        Guard.AgainstNull(options);

        if (string.IsNullOrWhiteSpace(options.ConnectionStringName))
        {
            return ValidateOptionsResult.Fail(Resources.ConnectionStringOptionException);
        }

        if (string.IsNullOrWhiteSpace(options.Schema))
        {
            return ValidateOptionsResult.Fail(Resources.SchemaOptionException);
        }

        return ValidateOptionsResult.Success;
    }
}