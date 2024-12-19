using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContext;

public class MyDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
        this.ChangeTracker.AutoDetectChangesEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // applies all entity configurations specified in types implementing IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        
        // seed data
    }

    public bool CheckConnection()
    {
        if (!this.Database.CanConnect()) return false;

        return true;
    }
    
    public DbSet<KanjiElement> KanjiElements { get; set; }
    public DbSet<ReadingElement> ReadingElements { get; set; }
    public DbSet<Sense> Senses { get; set; }
    public DbSet<Entry> Entries { get; set; }
    
    public DbSet<User> Users { get; set; }
}