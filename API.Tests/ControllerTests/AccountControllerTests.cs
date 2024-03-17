using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entitities;
using API.Helpers;
using API.Interfaces;
using API.Tests.ControllerTests.Utils;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
        private static string photoUrl = "https://testUrl.com";
        private static string token = "testToken";
        private static string password = "passw123";

        [SetUp]
        public void Init()
        {
            // in memory databse
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new DataContext(options);

            //add test data
            //context.Users.AddRange(TestDataUtils.GetAppUserList(6));

            //Dependencies
            tokenServiceMock = Substitute.For<ITokenService>();
            var myProfile = new AutoMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            mapper = new Mapper(configuration);
            userRepositoryMock = Substitute.For<IUserRepository>();

            tokenServiceMock.CreateToken(Arg.Any<AppUser>()).Returns(token);
            userRepositoryMock.GetUserMainPhotoUrl(Arg.Any<List<Photo>>()).Returns(photoUrl);

            //SUT - System Under Test
            this.accountController = new AccountController(
                context,
                tokenServiceMock,
                mapper,
                userRepositoryMock
            );
        }

        [Test]
        public async Task AccountController_Register_ValidDetails_ReturnsUserDto()
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
            userRepositoryMock.UserExistsAsync(Arg.Any<string>()).Returns(false);
            var expectedUser = new UserDto
            {
                UserName = user.UserName,
                Gender = user.Gender,
                KnownAs = user.KnownAs,
                Token = token
            };

            //Act
            var result = await accountController.Register(regDto);

            //Assert
            result.Should().BeOfType<ActionResult<UserDto>>();
            result.Result.Should().BeNull();
            result.Value.Should().Be(expectedUser);
        }

        [Test]
        public async Task AccountController_Register_EmptyRegisterDto_ReturnsBadRequest()
        {
            //Arrange
            RegisterDto regDto = new RegisterDto();
            userRepositoryMock.UserExistsAsync(Arg.Any<string>()).Returns(false);

            //Act
            var result = await accountController.Register(regDto);

            //Assert
            result.Should().BeOfType<ActionResult<UserDto>>();
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            result.Value.Should().BeNull();
        }

        [Test]
        public async Task AccountController_Login_CorrectCredentials_ReturnsUserDto()
        {
            //Arrange
            AppUser testUser = TestDataUtils.CreateAppUser(password);
            userRepositoryMock.GetUserByUsernameAsync(Arg.Any<string>()).Returns(testUser);
            LoginDto loginDto = new LoginDto { UserName = testUser.UserName, Password = password };
            UserDto expected = new UserDto
            {
                UserName = testUser.UserName,
                Token = token,
                PhotoUrl = photoUrl,
                KnownAs = testUser.KnownAs,
                Gender = testUser.Gender
            };

            //Act
            var result = await accountController.Login(loginDto);

            //Assert
            result.Should().BeOfType<ActionResult<UserDto>>();
            result.Result.Should().BeNull();
            result.Value.Should().Be(expected);
        }

        [Test]
        public async Task AccountController_Login_IncorrectPassword_ReturnsUnauthorized()
        {
            //Arrange
            AppUser testUser = TestDataUtils.CreateAppUser(password);
            userRepositoryMock.GetUserByUsernameAsync(Arg.Any<string>()).Returns(testUser);
            LoginDto loginDto = new LoginDto { UserName = testUser.UserName, Password = "invalid" };

            //Act
            var result = await accountController.Login(loginDto);

            //Assert
            result.Should().BeOfType<ActionResult<UserDto>>();
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
            if (result.Result != null)
            {
                var objectResult = (UnauthorizedObjectResult)result.Result;
                objectResult?.Value.Should().Be("invalid password");
            }
            result.Value.Should().BeNull();
        }

        [Test]
        public async Task AccountController_Login_IncorrectUsername_ReturnsUnauthorized()
        {
            //Arrange
            //not needed since we have incorrect username, so this method will return with null
            //userRepositoryMock.GetUserByUsernameAsync(Arg.Any<string>()).Returns(testUser);

            LoginDto loginDto = new LoginDto { UserName = "incorrect", Password = password };

            //Act
            var result = await accountController.Login(loginDto);

            //Assert
            result.Should().BeOfType<ActionResult<UserDto>>();
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
            if (result.Result != null)
            {
                var objectResult = (UnauthorizedObjectResult)result.Result;
                objectResult?.Value.Should().Be("invalid UserName");
            }
            result.Value.Should().BeNull();
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
