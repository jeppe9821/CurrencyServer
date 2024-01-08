using CurrencyServer.Middleware;
using CurrencyServer.Options;

namespace CurrencyServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddCurrencyService(builder.Configuration.GetSection(nameof(CurrencyServiceOptions)));

            builder.Services.AddExchangeRateHttpClient(builder.Configuration);

            builder.Services.ConfigureApiBehaviourOptions();

            var app = builder.Build();

            app.UseMiddleware<GenericExceptionHandlingMiddleware>();

            app.MapControllers();
            app.Run();
        }
    }
}

