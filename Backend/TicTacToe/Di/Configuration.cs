using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TicTacToe.Application;
using TicTacToe.Application.Interfaces;
using TicTacToe.DataSource;
using TicTacToe.Domain;

namespace TicTacToe.Di
{
    public class Configuration
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<TicTacToeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            //builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IServiceDomain, GameDomainService>();
            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
                });
        }
    }
}
