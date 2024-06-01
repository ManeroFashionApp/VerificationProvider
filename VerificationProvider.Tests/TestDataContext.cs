using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerificationProvider.Data.Contexts;

namespace VerificationProvider.Tests
{
    public class TestDataContext : IDbContextFactory<DataContext>
    {
        private readonly DbContextOptions<DataContext> _options;
        public TestDataContext(DbContextOptions<DataContext> options)
        {
            _options = options;
        }

        public DataContext CreateDbContext()
        {
            return new DataContext(_options);
        }



    }
}
