using Microsoft.Extensions.Logging;

namespace Infrastructure.Context.Initalization
{
    public class ApplicationDbSeeder
    {
        private readonly ILogger<ApplicationDbSeeder> _logger;
        private readonly ApplicationDbContext _db;
        private readonly CustomSeederRunner _seederRunner;

        public ApplicationDbSeeder(ILogger<ApplicationDbSeeder> logger, ApplicationDbContext db, CustomSeederRunner seederRunner)
        {
            _logger = logger;
            _db = db;
            _seederRunner = seederRunner;
        }

        public async Task SeedDatabaseAsync()
        {
            await SeedTypeAsync();
            await _seederRunner.RunSeedersAsync();
        }

        private async Task SeedTypeAsync()
        {
            if (!_db.Types.Any())
            {
                _logger.LogInformation("Seeding Type Service.");
                _db.Types.AddAsync(new Domain.Entities.Type
                {
                    Name = "Lựa chọn của biên tập viên"
                });
                _db.Types.AddAsync(new Domain.Entities.Type
                {
                    Name = "Mới nhất"
                });
                _db.Types.AddAsync(new Domain.Entities.Type
                {
                    Name = "Thịnh hành"
                });
                await _db.SaveChangesAsync();
            }
        }
    }
}