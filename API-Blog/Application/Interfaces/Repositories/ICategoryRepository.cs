using Domain.Entities;
using Application.Interfaces.Repositories.Common;

namespace Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<IEnumerable<Category>> SearchAsync(string keyword);
        Task<bool> ExistsByUrlSlugAsync(string urlSlug);
    }
}
