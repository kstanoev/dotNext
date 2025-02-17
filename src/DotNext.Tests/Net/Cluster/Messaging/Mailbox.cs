using System.Collections.Concurrent;
using System.Threading.Tasks;
using static Xunit.Assert;

namespace DotNext.Net.Cluster.Messaging
{
    internal sealed class Mailbox : ConcurrentQueue<StreamMessage>, IMessageHandler
    {
        async Task<IMessage> IMessageHandler.ReceiveMessage(IAddressee sender, IMessage message, object context)
        {
            Equal("Request", message.Name);
            Equal("text/plain", message.Type.MediaType);
            var text = await message.ReadAsTextAsync();
            Equal("Ping", text);
            return new TextMessage("Pong", "Reply");
        }

        async Task IMessageHandler.ReceiveSignal(IAddressee sender, IMessage signal, object context)
           => Enqueue(await StreamMessage.CreateBufferedMessageAsync(signal).ConfigureAwait(false));
    }
}