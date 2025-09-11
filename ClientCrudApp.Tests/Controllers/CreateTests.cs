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
    public class CreateTests
    {
        private readonly Mock<IClientRepository> _mockRepo;
        private readonly ClientController _controller;

        public CreateTests()
        {
            _mockRepo = new Mock<IClientRepository>();
            _controller = new ClientController(_mockRepo.Object);
            var tempData = new Mock<ITempDataDictionary>();
            _controller.TempData = tempData.Object;
        }

        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model); 
        }

        [Fact]
        public void Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange 
            var newClient = new Client
            {
                Id = 3,
                Name = "Hari",
                Gender = Client.GenderType.Male,
                CountryCode = "+977",
                Phone = "9841234567",
                Email = "hari@example.com",
                Address = "Lalitpur",
                Nationality = "Nepali",
                DateOfBirth = new DateTime(1999, 1, 15),
                EducationBackground = "BSc",
                PreferredModeOfContact = Client.ContactMode.Phone
            };

            // Act
            var result = _controller.Create(newClient);

            // Assert
            _mockRepo.Verify(r => r.AddClient(newClient), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void Create_Post_InvalidModel_ReturnsSameView()
        {
            // Arrange - intentionally invalid
            var newClient = new Client
            {
                Id = 4,
                Name = "",
                Gender = Client.GenderType.Male,
                CountryCode = "+977",
                Phone = "9800000000",
                Email = "invalid@example.com",
                Address = "Nowhere",
                Nationality = "Nepali",
                DateOfBirth = DateTime.Now,
                EducationBackground = "None",
                PreferredModeOfContact = Client.ContactMode.Email
            };
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = _controller.Create(newClient);

            // Assert
            _mockRepo.Verify(r => r.AddClient(It.IsAny<Client>()), Times.Never);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(newClient, viewResult.Model);
        }
    }
}
