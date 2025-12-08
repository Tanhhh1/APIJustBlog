using Application.Interfaces.Repositories;

namespace Application.Interfaces.UnitOfWork
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
        IUserRefreshTokenRepository UserRefreshTokenRepository { get; }
        
    }
}
