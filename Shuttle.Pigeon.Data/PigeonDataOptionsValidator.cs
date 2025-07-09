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

        return ValidateOptionsResult.Success;
    }
}