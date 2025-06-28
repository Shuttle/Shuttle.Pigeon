using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.Smtp;

public class SmtpOptionsValidator : IValidateOptions<SmtpOptions>
{
    public ValidateOptionsResult Validate(string? name, SmtpOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Host))
        {
            return ValidateOptionsResult.Fail("Option 'Host' may not be empty.");
        }

        if (string.IsNullOrWhiteSpace(options.Username))
        {
            return ValidateOptionsResult.Fail("Option 'Username' may not be empty.");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            return ValidateOptionsResult.Fail("Option 'Password' may not be empty.");
        }

        if (string.IsNullOrWhiteSpace(options.SenderAddress))
        {
            return ValidateOptionsResult.Fail("Option 'SenderAddress' may not be empty.");
        }

        if (string.IsNullOrWhiteSpace(options.SenderDisplayName))
        {
            return ValidateOptionsResult.Fail("Option 'SenderDisplayName' may not be empty.");
        }

        return ValidateOptionsResult.Success;
    }
}