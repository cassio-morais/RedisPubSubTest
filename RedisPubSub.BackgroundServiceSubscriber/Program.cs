using System.Security.Authentication;
using StackExchange.Redis;

namespace RedisPubSub.BackgroundServiceSubscriber
{
    public class Program
    {
        private static ConnectionMultiplexer _connection { get { return _lazyConnection.Value; } }

        private static Lazy<ConnectionMultiplexer> _lazyConnection = new(() =>
        {
            var redisConnectionString = "localhost:40551";
            // var redisConnectionString = "localhost:6379";

            return ConnectionMultiplexer.Connect(redisConnectionString, config =>
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
        });

        static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IConnectionMultiplexer>(_connection);
            builder.Services.AddHostedService<SubscriberService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }
    }
}