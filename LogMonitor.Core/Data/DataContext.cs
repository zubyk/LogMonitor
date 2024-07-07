using LogMonitor.Models;
using Microsoft.EntityFrameworkCore;

namespace LogMonitor.Data
{
    public abstract class DataContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<MonitoredResourceModel> Resources { get; set; }
        public DbSet<ResourceGroupModel> Groups { get; set; }

        protected DataContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}