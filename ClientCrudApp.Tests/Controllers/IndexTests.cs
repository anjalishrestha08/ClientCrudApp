using ClientCrudApp.Controllers;
using ClientCrudApp.Models;
using ClientCrudApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ClientCrudApp.Tests.Controllers
{
    public class IndexTests
    {
        private readonly Mock<IClientRepository> _mockRepo;
        private readonly Mock<ILogger<ClientController>> _mockLogger;
        private readonly ClientController _controller;
        private readonly List<Client> _sampleClients;

        public IndexTests()
        {
            _mockRepo = new Mock<IClientRepository>();
            _mockLogger = new Mock<ILogger<ClientController>>();
            _controller = new ClientController(_mockRepo.Object,_mockLogger.Object);
            var tempData = new Mock<ITempDataDictionary>();
            _controller.TempData = tempData.Object;

            // Sample Data
            _sampleClients = new List<Client>
            {
                new Client
                {
                    Id = 1,
                    Name = "Ram",
                    Gender = Client.GenderType.Male,
                    CountryCode = "+977",
                    Phone = "8273827193",
                    Email = "ram@example.com",
                    Address = "Jorpati",
                    Nationality = "Nepali",
                    DateOfBirth = new DateTime(2000,5,24),
                    EducationBackground = "BBS",
                    PreferredModeOfContact = Client.ContactMode.Email
                },
                new Client
                {
                    Id = 2,
                    Name = "Sita",
                    Gender = Client.GenderType.Female,
                    CountryCode = "+977",
                    Phone = "9812345678",
                    Email = "sita@example.com",
                    Address = "Bhaktapur",
                    Nationality = "Nepali",
                    DateOfBirth = new DateTime(2001, 12, 10),
                    EducationBackground = "BA",
                    PreferredModeOfContact = Client.ContactMode.Phone
                }
            };
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfClients()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAllClients()).Returns(_sampleClients);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // ensures result is ViewResult
            var model = Assert.IsAssignableFrom<IEnumerable<Client>>(viewResult.Model!); // model should not be null
            Assert.Equal(2, ((List<Client>)model).Count);
        }

        [Fact]
        public void Index_RedirectsToFirstPage_WhenPageLessThan1()
        {
            _mockRepo.Setup(r => r.GetAllClients()).Returns(_sampleClients);

            var result = _controller.Index(0);

            var redirect = Assert.IsType<RedirectToActionResult>(result); // ensures redirect
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal(1, redirect.RouteValues!["page"]);
        }

        [Fact]
        public void Index_RedirectsToLastPage_WhenPageGreaterThanTotal()
        {
            _mockRepo.Setup(r => r.GetAllClients()).Returns(_sampleClients);

            var result = _controller.Index(10);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal(1, redirect.RouteValues!["page"]);
        }

        [Fact]
        public void Index_ReturnsEmptyList_WhenNoClients()
        {
            _mockRepo.Setup(r => r.GetAllClients()).Returns(new List<Client>());

            var result = _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<Client>>(viewResult.Model!);
            Assert.Empty(model);
        }
    }
}
