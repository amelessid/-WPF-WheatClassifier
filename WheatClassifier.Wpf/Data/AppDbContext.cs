using Microsoft.EntityFrameworkCore;
using WheatClassifier.Wpf.Models;

namespace WheatClassifier.Wpf.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Experiment> Experiments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=experiments.db");
        }
    }
}