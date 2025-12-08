using Domain.Entities;
using Application.Interfaces.Repositories.Common;


namespace Application.Interfaces.Repositories
{
    public interface IPostTagMapRepository : IBaseRepository<PostTagMap>
    {
        Task<IEnumerable<PostTagMap>> GetByPostIdAsync(int postId);
    }
}
