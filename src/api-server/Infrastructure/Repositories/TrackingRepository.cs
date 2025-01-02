using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;

namespace Infrastructure.Repositories;

public class TrackingRepository: BaseRepository<EntryIsTagged>, ITrackingRepository
{
    private readonly MyDbContext _context;
    public TrackingRepository(MyDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task AddEntryToTag(string ent_seq, Guid TagId, Guid UserId)
    {
        throw new NotImplementedException();
    }
    
}