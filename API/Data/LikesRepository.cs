using API.DTOs;
using API.Entitities;
using API.Interfaces;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        public Task<AppUser> GerUserWithLikes(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LikeDto>> GetUserLikes(string predicate)
        {
            throw new NotImplementedException();
        }
    }
}
