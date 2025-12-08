using Domain.Entities;
using Application.Interfaces.Repositories;
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

        public async Task<bool> ExistsByUrlSlugAsync(string urlSlug)
        {
            return await _dbContext.Posts
                                 .AnyAsync(c => c.UrlSlug == urlSlug);
        }
    }
}
