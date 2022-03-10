using devSplain.Server;
using devSplain.Server.Controllers;
using devSplain.Server.Services;
using devSplain.Shared.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace devSplain.Tests.ServerTests
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        // Unit Test Mocks
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private List<UserModel> MockDb = new List<UserModel>();

        // Integration Test Client
        private readonly HttpClient _integrationClient;

        public UserControllerTests(WebApplicationFactory<Startup> fixture)
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
            _integrationClient = fixture.CreateClient();
        }

        /////////// MOCKS

        private List<UserModel> GetTestUsers()
        {
            List<UserModel> sessions = new List<UserModel>();
            sessions.Add(new UserModel()
            {
                FirstName = "Test",
                LastName = "User",
                UserId = "123"
            });
            sessions.Add(new UserModel()
            {
                FirstName = "Test",
                LastName = "User2",
                UserId = "456"
            });
            return sessions;
        }

        private UserModel GetTestUser(string id)
        {
            List<UserModel> sessions = new List<UserModel>();
            sessions.Add(new UserModel()
            {
                FirstName = "Test",
                LastName = "User",
                UserId = "123"
            });
            sessions.Add(new UserModel()
            {
                FirstName = "Test",
                LastName = "User2",
                UserId = "456"
            });
            return sessions.Find(x => x.UserId == id);
        }

        private Task AddTestUserToMockDb(UserModel user)
        {
            MockDb.Add(user);

            return Task.CompletedTask;
        }

        private UserModel GetTestUserFromMockDb(string id)
        {
            return MockDb.Find(x => x.UserId == id);
        }

        private Task EditTestUserInMockDb(UserModel user)
        {
            MockDb.Remove(user);
            MockDb.Add(user);

            return Task.CompletedTask;
        }

        /////////// UNIT TESTS
        [Fact]
        public async Task GetUsersUnitTest()
        {
            var mockDbService = new Mock<ICosmosDbService>();
            mockDbService.Setup(db => db.GetItemsAsync<UserModel>()).ReturnsAsync(GetTestUsers());
            var controller = new UserController(mockDbService.Object);

            var result = await controller.Get();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUserByIdUnitTest()
        {
            var UserId = "456";

            var mockDbService = new Mock<ICosmosDbService>();
            mockDbService.Setup(db => db.GetItemAsync<UserModel>(UserId)).ReturnsAsync(GetTestUser(UserId));
            var controller = new UserController(mockDbService.Object);

            var result = await controller.GetUser(UserId);

            Assert.Equal("Test", result.FirstName);
        }

        [Fact]
        public async Task CreateUserUnitTest()
        {
            UserModel user = new UserModel()
            {
                FirstName = "Test",
                LastName = "User",
                UserId = "123"
            };

            var id = user.UserId;

            var mockDbService = new Mock<ICosmosDbService>();

            // Create the user in mock db
            mockDbService.Setup(db => db.AddItemAsync<UserModel>(user, user.UserId)).Returns(AddTestUserToMockDb(user));

            // Get the user from the mock db
            mockDbService.Setup(db => db.GetItemAsync<UserModel>(user.UserId)).ReturnsAsync(GetTestUserFromMockDb(user.UserId));
            var controller = new UserController(mockDbService.Object);

            var createResult = await controller.CreateAsync(user);
            var getResult = await controller.GetUser(id);

            Assert.Equal("Test", getResult.FirstName);
        }

        [Fact]
        public async Task EditUserUnitTest()
        {
            UserModel user = new UserModel()
            {
                FirstName = "Test",
                LastName = "User",
                UserId = "123"
            };

            var id = user.UserId;

            var mockDbService = new Mock<ICosmosDbService>();

            // Create the user in mock db
            mockDbService.Setup(db => db.AddItemAsync<UserModel>(user, user.UserId)).Returns(AddTestUserToMockDb(user));

            // Get the user from the mock db
            mockDbService.Setup(db => db.GetItemAsync<UserModel>(user.UserId)).ReturnsAsync(GetTestUserFromMockDb(user.UserId));
            var controller = new UserController(mockDbService.Object);

            var createResult = await controller.CreateAsync(user);
            var getResult = await controller.GetUser(id);

            Assert.Equal("Test", getResult.FirstName);

            user.LastName = "Check";

            // Edit the user in mock db
            mockDbService.Setup(db => db.UpdateItemAsync<UserModel>(user.UserId, user)).Returns(EditTestUserInMockDb(user));

            // Get the user from the mock db
            mockDbService.Setup(db => db.GetItemAsync<UserModel>(user.UserId)).ReturnsAsync(GetTestUserFromMockDb(user.UserId));

            var editResult = await controller.EditAsync(user);
            var updatedResult = await controller.GetUser(user.UserId);

            Assert.Equal("Check", updatedResult.LastName);
        }
    }
}
