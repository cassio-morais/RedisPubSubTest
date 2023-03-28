using System.Security.Authentication;
using StackExchange.Redis;

namespace RedisPubSubTest.Subscriber
{
    internal class Program
    {
        private const string RedisConnectionString = "localhost:40551";
        //private const string RedisConnectionString = "localhost:6379";

        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(RedisConnectionString, config =>
        {
            config.User = "socket-notification";
            config.Password = "Ke4wue1UNg5uisha";
            config.AbortOnConnectFail = false;
            // SSL CONFIGURATION (cenário aws elasticache)
            config.Ssl = true;
            config.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            config.CheckCertificateRevocation = false;
            config.SslHost = string.Empty;
            // SSL BYPASS (cenário aws elasticache)
            config.CertificateValidation += (sender, certificate, chain, sslPolicyErrors) => true;
        });

        private const string Channel = "test-channel";

        static void Main(string[] args)
        {
            Console.WriteLine(connection.GetStatus());

            while (true)
            {
                try
                {
                    var subscriber = connection.GetSubscriber();
                    Console.WriteLine("PING:" + subscriber.Ping());

                    subscriber.SubscribeAsync(Channel, (channel, message) =>
                    {
                        Console.WriteLine("CONSOLE: MENSAGEM RECEBIDA <- test-channel : " + message);
                    });

                    Task.Delay(2000).Wait();
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
        }
    }
}