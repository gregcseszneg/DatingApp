using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entitities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext context;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public AccountController(
            DataContext context,
            ITokenService tokenService,
            IMapper mapper,
            IUserRepository userRepository
        )
        {
            this.tokenService = tokenService;
            this.context = context;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        [HttpPost("register")] // POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            try
            {
                if (await userRepository.UserExistsAsync(registerDto.UserName))
                {
                    return BadRequest("UserName is taken");
                }

                var user = mapper.Map<AppUser>(registerDto);

                using var hmac = new HMACSHA512(); //using keyword makes sure that this variable is disposed after we used it
                user.UserName = registerDto.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
                user.PasswordSalt = hmac.Key;

                this.context.Users.Add(user);
                await this.context.SaveChangesAsync();

                string token = tokenService.CreateToken(user);

                return new UserDto
                {
                    UserName = user.UserName,
                    Token = token,
                    KnownAs = user.KnownAs,
                    Gender = user.Gender
                };
            }
            catch (NullReferenceException err)
            {
                return BadRequest("The following paramter cannot be null: " + err);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userRepository.GetUserByUsernameAsync(loginDto.UserName);

            if (user == null)
            {
                return Unauthorized("invalid UserName");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("invalid password");
                }
            }

            string photoUrl = userRepository.GetUserMainPhotoUrl(user.Photos);
            string token = tokenService.CreateToken(user);

            return new UserDto
            {
                UserName = user.UserName,
                Token = token,
                PhotoUrl = photoUrl,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
    }
}
