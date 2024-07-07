using Microsoft.EntityFrameworkCore;

namespace LogMonitor.Data
{
    public class SqlServerDataContext : DataContext
    {
        public SqlServerDataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.SetCommandTimeout(Database.GetDbConnection().ConnectionTimeout);
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }
    }
}