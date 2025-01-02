using Domain.Entities;
using Domain.Entities.JMDict;
using Domain.Entities.Tracking;
using EntityFramework.Exceptions.PostgreSQL;
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
        modelBuilder.Entity<User>().HasData(
            new User {Id = new Guid("faeb2480-fbdc-4921-868b-83bd93324099"), Username = "myuser", Password = "password", Email = "myuser@mail.com", IsAdmin = false}
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor(); 
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
    
    public DbSet<Tag> Tags { get; set; }
    public DbSet<EntryIsTagged> EntryIsTagged { get; set; }
    public DbSet<TrackedEntry> TrackedEntries { get; set; }
    public DbSet<ReviewEvent> ReviewEvents { get; set; }
}