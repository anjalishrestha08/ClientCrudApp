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
    public class DeleteTests
    {
        private readonly Mock<IClientRepository> _mockRepo;
        private readonly ClientController _controller;

        public DeleteTests()
        {
            _mockRepo = new Mock<IClientRepository>();
            _controller = new ClientController(_mockRepo.Object);
            var tempData = new Mock<ITempDataDictionary>();
            _controller.TempData = tempData.Object;
        }

        // ----------------- GET -----------------
        [Fact]
        public void Delete_Get_ReturnsNotFound_WhenIdIsNull()
        {
            var result = _controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_Get_RedirectsToIndex_WhenClientNotFound()
        {
            _mockRepo.Setup(r => r.GetClientById(99)).Returns((Client?)null);

            var result = _controller.Delete(99);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void Delete_Get_ReturnsView_WithClient_WhenClientExists()
        {
            var client = new Client
            {
                Id = 1,
                Name = "Ram",
                Gender = Client.GenderType.Male,
                CountryCode = "+977",
                Phone = "9812345678",
                Email = "ram@example.com",
                Address = "Kathmandu",
                Nationality = "Nepali",
                DateOfBirth = new DateTime(2000, 5, 24),
                EducationBackground = "BBS",
                PreferredModeOfContact = Client.ContactMode.Email
            };

            _mockRepo.Setup(r => r.GetClientById(1)).Returns(client);

            var result = _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Client>(viewResult.Model);
            Assert.Equal("Ram", model.Name);
        }

        // ----------------- POST -----------------
        [Fact]
        public void DeleteConfirmed_Post_DeletesClient_AndRedirects()
        {
            // Arrange
            var clientId = 1;
            _mockRepo.Setup(r => r.DeleteClient(clientId)); // no return needed

            // Act
            var result = _controller.DeleteConfirmed(clientId);

            // Assert
            _mockRepo.Verify(r => r.DeleteClient(clientId), Times.Once);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void DeleteConfirmed_Post_ExceptionHandled_AndRedirects()
        {
            // Arrange
            var clientId = 1;
            _mockRepo.Setup(r => r.DeleteClient(clientId)).Throws(new Exception("CSV error"));

            // Act
            var result = _controller.DeleteConfirmed(clientId);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

           
        }
    }
}
