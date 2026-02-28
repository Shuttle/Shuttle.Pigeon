using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPigeon(Action<PigeonBuilder>? builder = null)
        {
            var messageBuilder = new PigeonBuilder(Guard.AgainstNull(services));

            builder?.Invoke(messageBuilder);

            services.TryAddSingleton<IValidateOptions<PigeonOptions>, PigeonOptionsValidator>();

            services.AddOptions<PigeonOptions>().Configure(options =>
            {
                options.ChannelDefaultMessageSenders = messageBuilder.Options.ChannelDefaultMessageSenders;
            });

            services.AddSingleton<IMessageService, MessageService>();

            return services;
        }
    }
}