using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Application.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync();
        Task DisposeAsync();
        Task BeginTransactionAsync();
        Task RollbackAsync();
        ICategoryRepository CategoryRepository { get; }
        ITagRepository TagRepository { get; }
        IPostRepository PostRepository { get; }
        IPostTagMapRepository PostTagMapRepository { get; }
    }
}
