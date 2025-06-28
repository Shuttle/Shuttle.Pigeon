using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.Postmark;

public class PostmarkOptionsValidator : IValidateOptions<PostmarkOptions>
{
    public ValidateOptionsResult Validate(string? name, PostmarkOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ServerToken))
        {
            return ValidateOptionsResult.Fail("Option 'ServerToken' must be provided.");
        }

        return ValidateOptionsResult.Success;
    }
}