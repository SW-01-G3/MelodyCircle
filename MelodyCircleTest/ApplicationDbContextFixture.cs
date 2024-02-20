using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelodyCircleTest
{
    public class ApplicationDbContextFixture : IDisposable
    {
        public DbContext DbContext { get; private set; }

        public ApplicationDbContextFixture()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<DbContext>()
                .UseSqlite(connection)
                .Options;
            DbContext = new DbContext(options);


            DbContext.Database.EnsureCreated();
        }


        public void Dispose() => DbContext.Dispose();
    }
}
