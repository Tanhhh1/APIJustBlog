using Domain.Entities;
using Application.Interfaces.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository(DatabaseContext dbContext) : BaseRepository<Category>(dbContext), ICategoryRepository
    {
        public async Task<bool> ExistsByUrlSlugAsync(string urlSlug)
        {
            return await _dbContext.Categories
                                 .AnyAsync(c => c.UrlSlug == urlSlug);
        }
    }
}
