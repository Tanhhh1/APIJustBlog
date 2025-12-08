using Domain.Entities;
using Application.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TagRepository(DatabaseContext dbContext) : BaseRepository<Tag>(dbContext), ITagRepository
    {
        public async Task<bool> ExistsByUrlSlugAsync(string urlSlug)
        {
            return await _dbContext.Tags
                                 .AnyAsync(c => c.UrlSlug == urlSlug);
        }
    }
}
