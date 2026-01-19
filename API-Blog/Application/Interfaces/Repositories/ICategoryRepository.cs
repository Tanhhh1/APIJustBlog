using Domain.Entities;
using Application.Interfaces.Repositories.Common;

namespace Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<bool> ExistsByUrlSlugAsync(string urlSlug);
    }
}
