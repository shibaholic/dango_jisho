using System.Text.RegularExpressions;
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
        // modelBuilder.Entity<Role>().HasData(
        //     new Role("ADMIN", new Guid("4764222f-8761-4991-bcd5-3ad618dc30c3")), 
        //     new Role("USER", new Guid("7101f179-1d38-4c08-bea0-54f6d9e05b5d"))
        // );
        //
        // modelBuilder.Entity<User>().HasData(
        //     new User(new Guid("faeb2480-fbdc-4921-868b-83bd93324099"),
        //         "MyAdmin",
        //         "$2a$11$f0x9GRd26NlxNw4R0eYJiu.wxQ6TTDg2QcSnG7aT1.IhgQ72B7gE6")
        // );
        
        // modelBuilder.Entity("UserRole").HasData([
        //     new { RoleId = new Guid("4764222f-8761-4991-bcd5-3ad618dc30c3"), UserId = new Guid("faeb2480-fbdc-4921-868b-83bd93324099") },
        //     new { RoleId = new Guid("7101f179-1d38-4c08-bea0-54f6d9e05b5d"), UserId = new Guid("faeb2480-fbdc-4921-868b-83bd93324099") }]
        // );
    }

    public bool CheckConnection()
    {
        if (!this.Database.CanConnect())
        {
            return false;
        }

        return true;
    }

    // public DbSet<User> Users { get; set; }
    // public DbSet<UserRegistrationToken> UserRegistrationTokens { get; set; }
    // public DbSet<Role> Roles { get; set; }
    // public DbSet<DataList> DataLists { get; set; }
    // public DbSet<Project> Projects { get; set; }
    // public DbSet<DataListRow> DataListRows { get; set; }
    // public DbSet<Match> Matches { get; set; }
    public DbSet<Entry> Entries { get; set; }
}