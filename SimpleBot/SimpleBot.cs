using Chatroom.NetworkClient.Bot;

namespace SimpleBot
{
    /// <summary>
    /// Sample implementation of a simple bot.
    /// </summary>
    public class SimpleBot : Bot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleBot"/> class.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        public SimpleBot(string hostname, ushort port)
            : base("Simpletron", hostname, port)
        {
        }

        /// <inheritdoc/>
        protected override void OnMessage(Message message)
        {
            if (message.Author.IsSelf)
            {
                return;
            }

            if (message.Content.StartsWith("deldet"))
            {
                Send("let Johnny = del det!");
            }
        }
    }
}
