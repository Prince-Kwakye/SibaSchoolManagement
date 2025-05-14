using Microsoft.EntityFrameworkCore;
using SibaSchoolManagementApi.Models;

namespace SibaSchoolManagementApi.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<SchoolDbContext>();

            
            await dbContext.Database.MigrateAsync();

           
            if (!await dbContext.Users.AnyAsync())
            {
                var users = new List<ApplicationUser>
                {
                    new()
                    {
                        Id = 1,
                        UserName = "admin",
                        PasswordHash = "$2a$12$uYxO/VpU7vRCGVrZyJYm/.DrbcAvbKuj8RMAWTQ94zLeBpqGgzvUq",
                        CustomRole = "Admin",
                        CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new()
                    {
                        Id = 2,
                        UserName = "staff",
                        PasswordHash = "$2a$12$fQHgWkNqORlj6jXgb9TTzOWKZUkMPTz3yW3LtDVoPUKZFFBk0hzqW",
                        CustomRole = "Staff",
                        CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
                };


                await dbContext.Users.AddRangeAsync(users);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
