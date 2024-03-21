using API.Controllers;
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

        [Test]
        public async Task UserController_SetMainPhoto_ValidIdButMain_WithMainPhoto_ReturnsBadRequest()
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
            var result = await userController.SetMainPhoto(1);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            if (result is BadRequestObjectResult)
            {
                (result as BadRequestObjectResult)?.StatusCode.Should().Be(400);
                (result as BadRequestObjectResult)
                    ?.Value.Should()
                    .Be("This is already your main photo");
            }
        }

        [Test]
        public async Task UserController_SetMainPhoto_InvalidId_WithMainPhoto_ReturnsNotFound()
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
            var result = await userController.SetMainPhoto(5);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
            if (result is NotFoundResult)
            {
                (result as NotFoundResult)?.StatusCode.Should().Be(404);
            }
        }

        [Test]
        public async Task UserController_SetMainPhoto_InvalidId_WithoutPhotos_ReturnsNotFound()
        {
            //Arrange
            AppUser user = TestDataUtils.CreateAppUser(password);

            userRepositoryMock.GetUserByUsernameAsync(null).Returns(user); //used null since there is no authenticated User
            userRepositoryMock.SaveAllAsync().Returns(true);

            //Act
            var result = await userController.SetMainPhoto(1);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
            if (result is NotFoundResult)
            {
                (result as NotFoundResult)?.StatusCode.Should().Be(404);
            }
        }

        [Test]
        public async Task UserController_SetMainPhoto_InvalidUsername_ReturnsNotFound()
        {
            //Arrange
            //*No mockking needed since teh default is that the GetUserByUsernameAsync return with null in this case

            //Act
            var result = await userController.SetMainPhoto(1);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
            if (result is NotFoundResult)
            {
                (result as NotFoundResult)?.StatusCode.Should().Be(404);
            }
        }
    }
}
