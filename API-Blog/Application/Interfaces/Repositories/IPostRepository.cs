using Domain.Entities;
using Application.Interfaces.Repositories.Common;

namespace Application.Interfaces.Repositories
{
    public interface IPostRepository : IBaseRepository<Post>
    {
        Task<IEnumerable<Post>> SearchAsync(string keyword);
        Task<bool> ExistsByUrlSlugAsync(string urlSlug);
    }
}
