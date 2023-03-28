using StackExchange.Redis;

namespace RedisPubSub.BackgroundServiceSubscriber
{
    public class SubscriberService : BackgroundService
    {
        private IConnectionMultiplexer _redis;
        private const string Channel = "test-channel";

        public SubscriberService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine(_redis.GetStatus() + "\b" + _redis.GetStormLog());

            try
            {
                var subscriber = _redis.GetSubscriber();

                await subscriber.SubscribeAsync(Channel, (channel, message) =>
                {
                    Console.WriteLine("PING:" + subscriber.Ping());
                    Console.WriteLine("BACKGROUND SERVICE: MENSAGEM RECEBIDA <- test-channel : " + message);
                });

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

