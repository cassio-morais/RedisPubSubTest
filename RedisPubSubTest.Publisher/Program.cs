using System.Security.Authentication;
using StackExchange.Redis;

namespace RedisPubSubTest.Publisher
{
    internal class Program
    {
         private const string RedisConnectionString = "localhost:40551";
        // private const string RedisConnectionString = "localhost:6379";

        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(RedisConnectionString, config =>
        {
            config.User = "socket-notification";
            config.Password = "lorota";
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
            var pubsub = connection.GetSubscriber();

            while (true)
            {
                try
                {
                    Console.WriteLine("PING:" + pubsub.Ping());

                    var randomNumber = new Random().Next();

                    pubsub.Publish(Channel, $"Essa é uma mensagem de teste! nº: {randomNumber}", CommandFlags.FireAndForget);

                    Console.WriteLine($"CONSOLE: MENSAGEM ENVIADA nº: {randomNumber} -> test-channel");

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