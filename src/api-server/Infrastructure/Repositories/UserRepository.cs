using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly MyDbContext _context;

    public UserRepository(MyDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<bool> AnyAsync(string username, CancellationToken cancelationToken)
    {
        return _context.Users
            .AnyAsync(x => x.Username == username, cancelationToken);
    }

    public Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return _context.Users
            .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public Task<User?> GetUserByRefreshCode(Guid refreshToken, CancellationToken cancellationToken)
    {
        return _context.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken, cancellationToken: cancellationToken);
    }

    public Task<bool> UpdateRefreshCodeToNull(Guid userId, Guid refreshToken, CancellationToken cancellationToken)
    {
        var user = _context.Users.FirstOrDefault(user => user.Id == userId && user.RefreshToken == refreshToken);

        if (user == null) return Task.FromResult(false);
        
        user.RefreshToken = null;

        _context.Update(user);

        return Task.FromResult(true);
    }
}