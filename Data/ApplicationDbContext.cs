using Common.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Common.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            //User
            builder.Entity<User>().HasData(GetPredefinedUsers());
            //Roles
            builder.Entity<IdentityRole<Guid>>().HasData(GetPredefinedRoles());
            //UserRoles
            builder.Entity<IdentityUserRole<Guid>>().HasData(GetPredefinedUserRoles());

        }

        private ICollection<IdentityRole<Guid>> GetPredefinedRoles()
        {
            return new List<IdentityRole<Guid>>()
            {
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("148fc39b-f404-43cc-9589-0b5c84e3a637"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "@stamp1"
                },
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("152a67b7-2a15-42d0-9d8e-675599dd4346"),
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "@stamp2"
                }
            };
        }

        private ICollection<User> GetPredefinedUsers()
        {
            var hasher = new PasswordHasher<User>();

            var admin = new User
            {
                Id = Guid.Parse("33f41895-b601-4aa1-8dc4-8229a9d07008"),
                UserName = "admin",
                FullName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@vqnofficial.com",
                NormalizedEmail = "ADMIN@VQNOFFICIAL.COM",
                EmailConfirmed = true,
                SecurityStamp = "@stamp3",
                ConcurrencyStamp = "@stamp4",
                PasswordHash = hasher.HashPassword(null!, "Aa@12345"),
               
            };

            var user = new User
            {
                Id = Guid.Parse("44f41895-b601-4aa1-8dc4-8229a9d07009"),
                UserName = "user",
                FullName = "User",
                NormalizedUserName = "USER",
                Email = "user@vqnofficial.com",
                NormalizedEmail = "USER@VQNOFFICIAL.COM",
                EmailConfirmed = true,
                SecurityStamp = "@stamp5",
                ConcurrencyStamp = "@stamp6",
                PasswordHash = hasher.HashPassword(null!, "Aa@12345")
            };

            return new List<User> { admin, user };
        }

        private ICollection<IdentityUserRole<Guid>> GetPredefinedUserRoles()
        {
            return new List<IdentityUserRole<Guid>>()
            {
                new IdentityUserRole<Guid>
                {
                    RoleId = Guid.Parse("148fc39b-f404-43cc-9589-0b5c84e3a637"), // Admin RoleId
                    UserId = Guid.Parse("33f41895-b601-4aa1-8dc4-8229a9d07008")  // Admin UserId
                },
                new IdentityUserRole<Guid>
                {
                    RoleId = Guid.Parse("152a67b7-2a15-42d0-9d8e-675599dd4346"), // User RoleId
                    UserId = Guid.Parse("44f41895-b601-4aa1-8dc4-8229a9d07009")  // User UserId
                }
            };
        }
    }
}