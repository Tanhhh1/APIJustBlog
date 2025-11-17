using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Common;

namespace Infrastructure.Repositories
{
    public class UserRefreshTokenRepository(DatabaseContext context) : BaseRepository<UserRefreshToken>(context), IUserRefreshTokenRepository;
}

