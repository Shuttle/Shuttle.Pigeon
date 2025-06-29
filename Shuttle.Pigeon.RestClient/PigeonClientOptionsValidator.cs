using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.RestClient;

public class PigeonClientOptionsValidator : IValidateOptions<PigeonClientOptions>
{
    public ValidateOptionsResult Validate(string? name, PigeonClientOptions options)
    {
        if (options.BaseAddress == null)
        {
            return ValidateOptionsResult.Fail("Option 'BaseAddress' must be provided.");
        }

        return ValidateOptionsResult.Success;
    }
}