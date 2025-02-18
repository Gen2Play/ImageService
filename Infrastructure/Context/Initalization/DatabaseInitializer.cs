using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Initalization
{
    public class DatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbSeeder _dbSeeder;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(ApplicationDbContext context, ApplicationDbSeeder dbSeeder, ILogger<DatabaseInitializer> logger)
        {
            _context = context;
            _dbSeeder = dbSeeder;
            _logger = logger;
        }
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (_context.Database.GetMigrations().Any())
            {
                if ((await _context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
                {
                    _logger.LogInformation("Applying Migrations for  tenant.");
                    await _context.Database.MigrateAsync(cancellationToken);
                }

                if (await _context.Database.CanConnectAsync(cancellationToken))
                {
                    _logger.LogInformation("Connection to Database Succeeded.");

                    await _dbSeeder.SeedDatabaseAsync();
                }
            }
        }
    }
}
