using System.Collections.Concurrent;
using System.Threading.Tasks;
using SyzunaPrograms.ExtensionBackendExample.HttpClients;
using SyzunaPrograms.ExtensionBackendExample.Models;

namespace SyzunaPrograms.ExtensionBackendExample.Services
{
    public class ExtensionPubsubService
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<ExtensionPubsubMessage>> _queues;
        private readonly TwitchExtensionHttpClient _extensionHttpClient;

        public ExtensionPubsubService(TwitchExtensionHttpClient extensionHttpClient)
        {
            _extensionHttpClient = extensionHttpClient;
            _queues = new ConcurrentDictionary<string, ConcurrentQueue<ExtensionPubsubMessage>>();

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await SendAsync();
                    await Task.Delay(1100);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public bool EnqueuePubsubMessage(ExtensionPubsubMessage message)
        {
            if (!_queues.ContainsKey(message.ChannelId))
            {
                if (!_queues.TryAdd(message.ChannelId, new ConcurrentQueue<ExtensionPubsubMessage>()))
                    return false;
            }

            if (!_queues.TryGetValue(message.ChannelId, out var queue))
                return false;

            queue.Enqueue(message);

            return true;
        }

        private async Task SendAsync()
        {
            foreach (var channel in _queues.Keys)
            {
                if (!_queues.TryGetValue(channel, out var queue))
                    return;
                if (!queue.TryDequeue(out var message))
                    return;

                await _extensionHttpClient.SendExtensionPubsubMessageAsync(message);
            }
        }
    }
}