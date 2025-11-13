using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Common;

namespace Infrastructure.Repositories
{
    public class TagRepository(DatabaseContext dbContext) : BaseRepository<Tag>(dbContext), ITagRepository
    {
    }
}
