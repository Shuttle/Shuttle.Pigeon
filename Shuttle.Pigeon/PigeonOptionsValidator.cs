using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon;

public class PigeonOptionsValidator : IValidateOptions<PigeonOptions>
{
    public ValidateOptionsResult Validate(string? name, PigeonOptions options)
    {
        foreach (var channelDefaultMessageSender in options.ChannelDefaultMessageSenders)
        {
            if (string.IsNullOrWhiteSpace(channelDefaultMessageSender.Channel))
            {
                return ValidateOptionsResult.Fail("All `ChannelDefaultMessageSender` options must have a `Channel` value.");
            }

            if (string.IsNullOrWhiteSpace(channelDefaultMessageSender.Name))
            {
                return ValidateOptionsResult.Fail("All `ChannelDefaultMessageSender` options must have a `Name` value.");
            }
        }

        var duplicateChannels = options.ChannelDefaultMessageSenders
            .GroupBy(s => s.Channel)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateChannels.Any())
        {
            return ValidateOptionsResult.Fail($"There is more than 1 `ChannelDefaultMessageSender` entry for channel '{duplicateChannels.First()}'.");
        }

        return ValidateOptionsResult.Success;
    }
}