﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using YoApp.Identity.Controllers;
using YoApp.Friends.Helper;
using Microsoft.AspNetCore.Http;
using YoApp.Data.Models;
using System.Threading.Tasks;
using YoApp.DataObjects.Users;
using System.Collections.Generic;
using System.Linq;

namespace YoApp.Tests.Services.Friends.Controller
{
    public class FriendsControllerTests
    {
        private ILogger<FriendsController> _logger;

        public FriendsControllerTests()
        {
            _logger = new Mock<ILogger<FriendsController>>().Object;
        }

        [Fact]
        public async void FindFriend_OnEmptyPhoneNumber_ReturnsBadRequest()
        {
            //Arrange
            var mapperMock = new Mock<IMapper>();
            var persistenceMock = new Mock<IFriendsPersistence>();

            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.FindUser(string.Empty);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Theory]
        [InlineData("Bob")]
        public async void FindFriend_OnUnknownPhoneNumber_ReturnsNotFound(string phoneNumber)
        {
            //Arrange
            var mapperMock = new Mock<IMapper>();
            var persistenceMock = new Mock<IFriendsPersistence>();
            persistenceMock
                .Setup(r => r.Friends
                .FindByNameAsync(phoneNumber))
                .Returns(Task.FromResult<ApplicationUser>(null));

            var httpContextMock = new Mock<DefaultHttpContext>(null);
            httpContextMock
                .SetupGet(am => am.User.Identity.Name)
                .Returns("Somebody");

            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);
            controller.ControllerContext.HttpContext = httpContextMock.Object;

            //Act
            var respone = await controller.FindUser(phoneNumber);

            //Assert
            Assert.IsType<NotFoundResult>(respone);
        }

        [Theory]
        [InlineData("Bob")]
        public async void FindFriend_OnOkResponse_ReturnsOkWithUserDtoWithValidProperties(string phoneNumber)
        {
            //Arrange
            var fakeUser = new ApplicationUser { UserName = phoneNumber };
            var fakeDto = new UserDto { Username = phoneNumber };

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(m => m.Map<UserDto>(fakeUser))
                .Returns(fakeDto);

            var persistenceMock = new Mock<IFriendsPersistence>();
            persistenceMock
                .Setup(r => r.Friends
                .FindByNameAsync(phoneNumber))
                .ReturnsAsync(fakeUser);

            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var respone = await controller.FindUser(phoneNumber);
            var dto = ((OkObjectResult)respone).Value as UserDto;

            //Assert
            Assert.IsType<OkObjectResult>(respone);
            Assert.IsType<UserDto>(dto);
            Assert.Equal(phoneNumber, dto.Username);
        }

        [Fact]
        public async void FindFriends_OnAnyFindingMatches_ReturnsOkWithMatchingUserDtos()
        {
            //Arrange
            var requestPhoneNumbers = new List<string> { "123", "456", "789" };

            var usersInDb = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "123"},
                new ApplicationUser { UserName = "456" }
            };

            var fakeDtos = new List<UserDto>();
            usersInDb.ForEach(a => fakeDtos.Add(new UserDto { Username = a.UserName }));

            var persistenceMock = new Mock<IFriendsPersistence>();
            persistenceMock
                .Setup(r => r.Friends
                .FindByNameRangeAsync(requestPhoneNumbers))
                .ReturnsAsync(usersInDb);

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(m => m.Map<IEnumerable<UserDto>>(usersInDb))
                .Returns(fakeDtos);

            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.FindUsers(requestPhoneNumbers);
            var dtos = ((OkObjectResult)response).Value as IEnumerable<UserDto>;

            //Assert
            Assert.IsType<OkObjectResult>(response);
            Assert.Equal(usersInDb.Count, dtos.Count());
        }

        [Fact]
        public async void FindFriends_OnInvalidQuery_BadRequest()
        {
            //Arrange
            var persistenceMock = new Mock<IFriendsPersistence>();
            var mapperMock = new Mock<IMapper>();
            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.FindUsers(null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void GetName_OnEmptyQuery_BadRequest()
        {
            //Arrange
            var persistenceMock = new Mock<IFriendsPersistence>();
            var mapperMock = new Mock<IMapper>();
            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.GetName(null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void GetName_OnValidQuery_OkRequestWithName()
        {
            //Arrange
            var query = "0123456789";
            var dto = new ApplicationUser { UserName = query, Nickname = "I'm Mock!" };

            var persistenceMock = new Mock<IFriendsPersistence>();
            persistenceMock
                .Setup(p => p.Friends.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(dto);

            var mapperMock = new Mock<IMapper>();
            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.GetName(query);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async void GetStatus_OnEmptyQuery_BadRequest()
        {
            //Arrange
            var persistenceMock = new Mock<IFriendsPersistence>();
            var mapperMock = new Mock<IMapper>();
            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.GetStatus(null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void GetStatus_OnValidQuery_OkRequestWithStatus()
        {
            //Arrange
            var query = "0123456789";
            var dto = new ApplicationUser { UserName = query, Status = "I'm mocking you!" };

            var persistenceMock = new Mock<IFriendsPersistence>();
            persistenceMock
                .Setup(p => p.Friends.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(dto);

            var mapperMock = new Mock<IMapper>();
            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.GetStatus(query);

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async void CheckIsMember_OnEmptyQuery_BadRequest()
        {
            //Arrange
            var persistenceMock = new Mock<IFriendsPersistence>();
            var mapperMock = new Mock<IMapper>();
            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.IsMember(null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void CheckIsMember_OnValidQuery_OkRequest()
        {
            //Arrange
            var query = "123456789";

            var persistenceMock = new Mock<IFriendsPersistence>();
            persistenceMock
                .Setup(p => p.Friends.IsMemberAsync(query))
                .ReturnsAsync(true);

            var mapperMock = new Mock<IMapper>();
            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);
            var httpContextMock = new Mock<DefaultHttpContext>(null);
            httpContextMock
                .SetupGet(am => am.User.Identity.Name)
                .Returns(query);

            controller.ControllerContext.HttpContext = httpContextMock.Object;

            //Act
            var response = await controller.IsMember(query);

            //Assert
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async void CheckAreMember_OnEmptyQuery_BadRequest()
        {
            //Arrange
            var persistenceMock = new Mock<IFriendsPersistence>();
            var mapperMock = new Mock<IMapper>();
            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.IsMemberRange(null);

            //Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async void CheckAreMember_OnAnyFindingMatches_OkResultWithDto()
        {
            //Arrange
            var query = new List<string> { "123", "456", "789" };
            var usersFromDb = new List<ApplicationUser> { new ApplicationUser { UserName = query[0] } };
            var dtos = new List<UserDto> { new UserDto { Username = usersFromDb[0].UserName } };

            var persistenceMock = new Mock<IFriendsPersistence>();
            persistenceMock
                .Setup(p => p.Friends.FindByNameRangeAsync(query))
                .ReturnsAsync(usersFromDb);

            var mapperMock = new Mock<IMapper>();
            mapperMock
                .Setup(m => m.Map<IEnumerable<ApplicationUser>, IEnumerable<UserDto>>(usersFromDb))
                .Returns(dtos);

            var controller = new FriendsController(_logger, persistenceMock.Object, mapperMock.Object);

            //Act
            var response = await controller.IsMemberRange(query);
            
            //Assert
            Assert.IsType<OkObjectResult>(response);
        }
    }
}
