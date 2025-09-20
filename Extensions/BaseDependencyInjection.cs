using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class BaseDependencyInjection
    {
        public static IServiceCollection AddBaseServicesWithDbContext<TDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TDbContext : DbContext
        {
            var connectionString = configuration["DATABASE_CONNECTION_STRING"];
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Warning: DATABASE_CONNECTION_STRING is not set in environment variables.");
            }

            services.AddDbContext<TDbContext>(options =>
                options.UseSqlServer(connectionString)
                       .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            return services;
        }
    }

}