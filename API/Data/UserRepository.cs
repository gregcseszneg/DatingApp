using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entitities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        public readonly IMapper Mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            this.Mapper = mapper;
            this.context = context;
            
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await this.context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(this.Mapper.ConfigurationProvider)  //project AppUser into MemberDto using automapper, get the config for automapper from our service where we added it
            .SingleOrDefaultAsync();        
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await this.context.Users
            .ProjectTo<MemberDto>(this.Mapper.ConfigurationProvider)
            .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await this.context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await this.context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await this.context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await this.context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            this.context.Entry(user).State = EntityState.Modified;
        }
    }
}