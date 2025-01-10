using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudySetRepository : BaseRepository<StudySet>, IStudySetRepository
{
    private readonly MyDbContext _context;
    public StudySetRepository(MyDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<StudySet?> ReadByIdUserId(Guid id, Guid userId)
    {
        return await _context.StudySets.Where(ss => ss.UserId == userId && ss.Id == id).FirstOrDefaultAsync();
    }
    
    public async Task<List<TrackedEntry>> GetTrackedEntriesByStudySet(Guid studySetId)
    {
        var query = _context.TagInStudySets
            .OrderBy(tis => tis.Order)
            .SelectMany(tis => tis.Tag.EntryIsTaggeds
                .Where(eit => eit.TagId == tis.TagId)
                .OrderByDescending(eit => eit.UserOrder)
            )
            .Select(eit => eit.TrackedEntry);

        return await query.ToListAsync();
    }
}