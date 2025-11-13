using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostRepository(DatabaseContext dbContext) : BaseRepository<Post>(dbContext), IPostRepository
    {
        public async Task<IEnumerable<Post>> SearchAsync(string keyword)
        {
            return await _dbContext.Posts
                .Where(c => c.Title.Contains(keyword) || c.ShortDescription.Contains(keyword))
                .ToListAsync();
        }
    }
}
