using Microsoft.EntityFrameworkCore;
using QuickRecord.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QuickRecord.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<QuickNote> QuickNotes => Set<QuickNote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 建索引以加速搜尋
            modelBuilder.Entity<QuickNote>()
                        .HasIndex(q => q.CreatedUtc);

            base.OnModelCreating(modelBuilder);
        }
    }
}
