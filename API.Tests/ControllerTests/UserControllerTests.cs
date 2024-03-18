using System.Security.Claims;
using API.Controllers;
using API.Data;
using API.Entitities;
using API.Helpers;
using API.Interfaces;
using API.Tests.ControllerTests.Utils;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace API.Tests.ControllerTests
{
    [TestFixture]
    public class UserControllerTests
    {
        private UsersController userController;
        private IUserRepository userRepositoryMock;
        private IPhotoService photoServiceMock;
        private IMapper mapper;
        private static string password = "passw123";

        [SetUp]
        public void init()
        {
            userRepositoryMock = Substitute.For<IUserRepository>();
            var myProfile = new AutoMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            mapper = new Mapper(configuration);
            photoServiceMock = Substitute.For<IPhotoService>();
            userController = new UsersController(userRepositoryMock, mapper, photoServiceMock);
        }

        [Test]
        public async Task UserController_SetMainPhoto_ValidId_WithMainPhoto_ReturnsNoContent()
        {
            //Arrange
            AppUser user = TestDataUtils.CreateAppUser(password);
            user.Photos.AddRange(
                new List<Photo>
                {
                    new Photo
                    {
                        Id = 1,
                        PublicId = "2",
                        IsMain = true,
                        Url = "https://uploader.com/randomPic2",
                        AppUserId = user.Id,
                        AppUser = user
                    },
                    new Photo
                    {
                        Id = 2,
                        PublicId = "3",
                        IsMain = false,
                        Url = "https://uploader.com/randomPic3",
                        AppUserId = user.Id,
                        AppUser = user
                    }
                }
            );
            userRepositoryMock.GetUserByUsernameAsync(null).Returns(user); //used null since there is no authenticated User
            userRepositoryMock.SaveAllAsync().Returns(true);

            //Act
            var result = await userController.SetMainPhoto(2);

            //Assert
            result.Should().BeOfType<NoContentResult>();
            if (result is NoContentResult)
            {
                (result as NoContentResult)?.StatusCode.Should().Be(204);
            }
        }
    }
}
