using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Initalization
{
    public class CustomSeederRunner
    {
        private readonly ICustomSeeder[] _seeders;

        public CustomSeederRunner(IServiceProvider serviceProvider) =>
            _seeders = serviceProvider.GetServices<ICustomSeeder>().ToArray();

        public async Task RunSeedersAsync()
        {
            foreach (var seeder in _seeders)
            {
                await seeder.InitializeAsync();
            }
        }
    }
}
