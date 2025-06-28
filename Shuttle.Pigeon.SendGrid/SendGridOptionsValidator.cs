using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.SendGrid;

public class SendGridOptionsValidator : IValidateOptions<SendGridOptions>
{
    public ValidateOptionsResult Validate(string? name, SendGridOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return ValidateOptionsResult.Fail("Option 'ApiKey' must be provided.");
        }

        return ValidateOptionsResult.Success;
    }
}