using Domain.Entities;
using Infrastructure.Interfaces.Common;

namespace Infrastructure.Interfaces
{
    public interface IPostRepository : IBaseRepository<Post>
    {
        Task<IEnumerable<Post>> SearchAsync(string keyword);
    }
}
