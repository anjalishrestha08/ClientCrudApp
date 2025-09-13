using ClientCrudApp.Controllers;
using ClientCrudApp.Models;
using ClientCrudApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace ClientCrudApp.Tests.Controllers
{
    public class EditTests
    {
        private readonly Mock<IClientRepository> _mockRepo;
        private readonly Mock<ILogger<ClientController>> _mockLogger;
        private readonly ClientController _controller;

        public EditTests()
        {
            _mockRepo = new Mock<IClientRepository>();
            _mockLogger = new Mock<ILogger<ClientController>>();
            _controller = new ClientController(_mockRepo.Object, _mockLogger.Object);
            var tempData = new Mock<ITempDataDictionary>();
            _controller.TempData = tempData.Object;
        }

        // ----------------- GET -----------------
        [Fact]
        public void Edit_Get_ReturnsNotFound_WhenIdIsNull()
        {
            var result = _controller.Edit(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Get_RedirectsToIndex_WhenClientNotFound()
        {
            _mockRepo.Setup(r => r.GetClientById(99)).Returns((Client?)null);

            var result = _controller.Edit(99);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void Edit_Get_ReturnsView_WithClient_WhenClientExists()
        {
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

            var result = _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Client>(viewResult.Model);
            Assert.Equal("Ram", model.Name);
        }

        // ----------------- POST -----------------
        [Fact]
        public void Edit_Post_ReturnsNotFound_WhenIdMismatch()
        {
            var client = new Client {
                Id = 2,
                Name = "Mismatch",
                Gender = Client.GenderType.Male,
                CountryCode = "+977",
                Phone = "9812345678",
                Email = "mismatch@example.com",
                Address = "Lalitpur",
                Nationality = "Nepali",
                DateOfBirth = new DateTime(1995, 5, 1),
                EducationBackground = "BSc",
                PreferredModeOfContact = Client.ContactMode.Email
            };

            var result = _controller.Edit(1, client);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Post_ValidModel_RedirectsToIndex()
        {
            var client = new Client
            {
                Id = 1,
                Name = "Hari",
                Gender = Client.GenderType.Male,
                CountryCode = "+977",
                Phone = "9812345678",
                Email = "hari@example.com",
                Address = "Lalitpur",
                Nationality = "Nepali",
                DateOfBirth = new DateTime(1999, 1, 15),
                EducationBackground = "BSc",
                PreferredModeOfContact = Client.ContactMode.Phone
            };

            var result = _controller.Edit(1, client);

            _mockRepo.Verify(r => r.UpdateClient(client), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void Edit_Post_InvalidModel_ReturnsSameView()
        {
            var client = new Client {
                Id = 1,
                Name = "", // invalid
                Gender = Client.GenderType.Male,
                CountryCode = "+977",
                Phone = "9812345678",
                Email = "invalid@example.com",
                Address = "Lalitpur",
                Nationality = "Nepali",
                DateOfBirth = new DateTime(2000, 1, 1),
                EducationBackground = "BSc",
                PreferredModeOfContact = Client.ContactMode.Email
            }; // invalid
            _controller.ModelState.AddModelError("Name", "Required");

            var result = _controller.Edit(1, client);

            _mockRepo.Verify(r => r.UpdateClient(It.IsAny<Client>()), Times.Never);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(client, viewResult.Model);
        }
    }
}
