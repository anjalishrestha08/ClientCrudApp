using ClientCrudApp.Controllers;
using ClientCrudApp.Models;
using ClientCrudApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using Xunit;

namespace ClientCrudApp.Tests.Controllers
{
    public class DetailsTests
    {
        private readonly Mock<IClientRepository> _mockRepo;
        private readonly ClientController _controller;

        public DetailsTests()
        {
            _mockRepo = new Mock<IClientRepository>();
            _controller = new ClientController(_mockRepo.Object);
            var tempData = new Mock<ITempDataDictionary>();
            _controller.TempData = tempData.Object;
        }

        [Fact]
        public void Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = _controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Details_RedirectsToIndex_WhenClientNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetClientById(99)).Returns((Client?)null);

            // Act
            var result = _controller.Details(99);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void Details_ReturnsViewResult_WithClient_WhenClientExists()
        {
            // Arrange - valid client
            var client = new Client
            {
                Id = 1,
                Name = "Ram",
                Gender = Client.GenderType.Male,
                CountryCode = "+977",
                Phone = "8273827193",
                Email = "ram@example.com",
                Address = "Jorpati",
                Nationality = "Nepali",
                DateOfBirth = new DateTime(2000, 5, 24),
                EducationBackground = "BBS",
                PreferredModeOfContact = Client.ContactMode.Email
            };
            _mockRepo.Setup(r => r.GetClientById(1)).Returns(client);

            // Act
            var result = _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Client>(viewResult.Model);
            Assert.Equal("Ram", model.Name);
            Assert.Equal(1, model.Id);
        }
    }
}
