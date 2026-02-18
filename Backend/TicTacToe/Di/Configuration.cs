using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TicTacToe.Application;
using TicTacToe.Application.Interfaces;
using TicTacToe.DataSource;
using TicTacToe.DataSource.Auth;
using TicTacToe.Domain;

namespace TicTacToe.Di
{
    public class Configuration
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<TicTacToeDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IServiceDomain, GameDomainService>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
                });
            builder.Services.AddAuthentication("Basic")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
            builder.Services.AddAuthorization();
        }
    }

}
