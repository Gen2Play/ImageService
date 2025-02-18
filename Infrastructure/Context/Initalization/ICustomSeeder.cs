using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Initalization
{
    public interface ICustomSeeder
    {
        Task InitializeAsync();
    }
}
