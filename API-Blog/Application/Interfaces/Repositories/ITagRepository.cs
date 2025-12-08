using Domain.Entities;
using Application.Interfaces.Repositories.Common;

namespace Application.Interfaces.Repositories
{
    public interface ITagRepository : IBaseRepository<Tag>
    {
        Task<bool> ExistsByUrlSlugAsync(string urlSlug);
    }
}
