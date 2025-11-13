using Domain.Entities;
using Infrastructure.Interfaces.Common;

namespace Infrastructure.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<IEnumerable<Category>> SearchAsync(string keyword);
    }
}
