
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

namespace RedisCacheExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Fetch connection string from appsettings.json
            var redisConnectionString = builder.Configuration.GetConnectionString("RedisCache")
                ?? throw new InvalidOperationException("Redis connection string is missing.");

            var redisConnString = ConnectionMultiplexer.Connect(redisConnectionString);
            // Requires Microsoft.AspNetCore.DataProtection.StackExchangeRedis
            builder.Services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redisConnString, "DataProtection-Keys")
                .SetApplicationName("SharedWebApiApp");

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                // Prefixing all keys prevents workspace data collisions in shared Redis environments
                options.InstanceName = "ProdWebApi_";
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                // Set a strict, uniform cookie name for your entire multi-instance farm
                options.Cookie.Name = ".Prod.SessionIdentifier";
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllers();

            app.Run();
        }
    }
}
