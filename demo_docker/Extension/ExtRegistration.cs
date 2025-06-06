using demo_docker.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_docker.Extension
{
    public static class ExtRegistration
    {
        public static void ApplyMigration(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.Migrate();
        }

    }
}
