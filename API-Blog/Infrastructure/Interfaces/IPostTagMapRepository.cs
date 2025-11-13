using Domain.Entities;
using Infrastructure.Interfaces.Common;


namespace Infrastructure.Interfaces
{
    public interface IPostTagMapRepository : IBaseRepository<PostTagMap>
    {
        Task<IEnumerable<PostTagMap>> GetByPostIdAsync(int postId);
    }
}
