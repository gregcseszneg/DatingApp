using System;
using System.Text;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entitities;
using API.Helpers;
using API.Interfaces;
using API.Tests.ControllerTests.Utils;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace API.Tests.ControllerTests
{
    [TestFixture]
    public class AccountControllerTests
    {
        private DataContext context;
        private ITokenService tokenServiceMock;
        private IMapper mapper;
        private AccountController accountController;
        private IUserRepository userRepositoryMock;

        [SetUp]
        public void Init()
        {
            // in memory databse
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new DataContext(options);

            //add test data
            context.Users.AddRange(TestDataUtils.getAppUserList(6));

            //Dependencies
            tokenServiceMock = Substitute.For<ITokenService>();
            var myProfile = new AutoMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            mapper = new Mapper(configuration);
            userRepositoryMock = Substitute.For<IUserRepository>();

            //SUT - System Under Test
            this.accountController = new AccountController(
                context,
                tokenServiceMock,
                mapper,
                userRepositoryMock
            );
        }

        [Test]
        public async Task AccounTController_Register_ReturnsUserDto()
        {
            //Arrange
            RegisterDto regDto = new RegisterDto
            {
                UserName = Faker.Internet.UserName(),
                City = Faker.Address.City(),
                Country = Faker.Address.Country(),
                DateOfBirth = new DateOnly(2001, 03, 28),
                Gender = "Male",
                KnownAs = Faker.Name.First(),
                Password = "test123"
            };
            var user = mapper.Map<AppUser>(regDto);
            userRepositoryMock.UserExistsAsync(regDto.UserName).Returns(false);
            tokenServiceMock.CreateToken(Arg.Any<AppUser>()).Returns("testToken");
            var expectedUser = new UserDto
            {
                UserName = user.UserName,
                Gender = user.Gender,
                KnownAs = user.KnownAs,
                Token = "testToken"
            };

            //Act
            var result = await accountController.Register(regDto);

            //Assert
            result.Should().BeOfType<ActionResult<UserDto>>();

            var addedItem = await context.Users.FirstOrDefaultAsync(u =>
                u.UserName == regDto.UserName
            );

            addedItem.Should().NotBeNull();
            result.Value.Should().Be(expectedUser);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
