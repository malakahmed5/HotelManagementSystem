using HMS.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HMS.API.WebApplicationRegister
{
    public static class WebApplicationRegister
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
                await dbContext.Database.MigrateAsync();

            return app;
        }
    }
}
