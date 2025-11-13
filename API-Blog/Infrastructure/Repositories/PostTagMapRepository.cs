using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostTagMapRepository(DatabaseContext dbContext) : BaseRepository<PostTagMap>(dbContext), IPostTagMapRepository
    {
        public async Task<IEnumerable<PostTagMap>> GetByPostIdAsync(int postId)
        {
            return await _dbContext.Set<PostTagMap>()
                .Include(pt => pt.Tag)
                .Where(pt => pt.PostId == postId)
                .ToListAsync();
        }
    }
}
