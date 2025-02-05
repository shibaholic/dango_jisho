using FluentAssertions;
using Infrastructure.DbContext;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tests.IntegrationTests.RepositoryIntegrationTests;

public class EntryRepositoryIntegrationTests : IDisposable
{
    private readonly MyDbContext _dbContext;
    private readonly EntryRepository _entryRepository;

    public EntryRepositoryIntegrationTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<EntryRepositoryIntegrationTests>()
            .Build();
        
        var connectionString = configuration["ConnectionStrings:PostgreSQL"];
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string is missing from secrets.");

        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        
        _dbContext = new MyDbContext(options);
        
        _entryRepository = new EntryRepository(_dbContext);
    }

    [Fact]
    public async Task BulkReadAllAsync_ShouldReturnSameResults_AsEfCoreReadAll()
    {
        // Arrange
        
        // Act
        var entriesBulk = await _entryRepository.BulkReadAllAsync();
        var entriesEfCore = (await _entryRepository.ReadAllAsync()).ToList();

        foreach (var entry in entriesEfCore)
        {
            entry.KanjiElements.ForEach(k => k.Entry = null);
            entry.ReadingElements.ForEach(k => k.Entry = null);
            entry.Senses.ForEach(k => k.Entry = null);
            entry.Senses.ForEach(s => s.lsource.ForEach(l => l.Sense = null));
        }
        
        // Assert
        entriesBulk.Should().HaveSameCount(entriesEfCore);
        entriesBulk.Should().BeEquivalentTo(entriesEfCore);
    }
    
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}