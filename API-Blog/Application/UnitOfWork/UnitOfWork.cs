using Infrastructure.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _dbContext;
        private IDbContextTransaction? _transaction;
        private ICategoryRepository? _categoryRepository;
        private IPostRepository? _postRepository;
        private ITagRepository? _tagRepository;
        private IPostTagMapRepository? _postTagMapRepository;
        private IUserRefreshTokenRepository? _userRefreshTokenRepository;
        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(_dbContext);
        public IPostRepository PostRepository => _postRepository ??= new PostRepository(_dbContext);
        public ITagRepository TagRepository => _tagRepository ??= new TagRepository(_dbContext);
        public IPostTagMapRepository PostTagMapRepository => _postTagMapRepository ??= new PostTagMapRepository(_dbContext);
        public IUserRefreshTokenRepository UserRefreshTokenRepository => _userRefreshTokenRepository ??= new UserRefreshTokenRepository(_dbContext);

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_dbContext.Database.CurrentTransaction == null)
            {
                _transaction = await _dbContext.Database.BeginTransactionAsync();
            }
            else
            {
                _transaction = _dbContext.Database.CurrentTransaction;
            }
        }

    }
}
