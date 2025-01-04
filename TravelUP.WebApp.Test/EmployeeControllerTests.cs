using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TravelUp.EmployeeAPI.Client.Interfaces;
using TravelUp.EmployeeAPI.Models;
using TravelUp.WebApp.Controllers;

namespace TravelUP.WebApp.Test
{

    [TestClass]
    public class EmployeeControllerTests
    {
        private IEmployeeApiService _employeeProviderMock;
        private IMapper _mapperMock;
        private Microsoft.Extensions.Logging.ILogger<EmployeeController> _loggerMock;
        private EmployeeController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _employeeProviderMock = MockRepository.GenerateMock<IEmployeeApiService>();
            _mapperMock = MockRepository.GenerateMock<IMapper>();
            _loggerMock = MockRepository.GenerateMock<Microsoft.Extensions.Logging.ILogger<EmployeeController>>();
            _controller = new EmployeeController(_employeeProviderMock, _mapperMock, _loggerMock);
        }

        [TestMethod]
        public async Task Index_ShouldReturnViewResult()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [TestMethod]
        public async Task Create_Post_ShouldRedirectToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var employee = new TravelUp.WebApp.Models.Employee { Id = Guid.NewGuid(), Name = "John Doe" };
            _employeeProviderMock.Stub(p => p.CreateEmployeeAsync(Arg<Employee>.Is.Anything))
                                 .Return(Task.CompletedTask);

            // Act
            var result = await _controller.Create(employee);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual(nameof(EmployeeController.Index), redirectResult.ActionName);
        }

        [TestMethod]
        public async Task DeleteConfirmed_ShouldRedirectToIndex()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _employeeProviderMock.Stub(p => p.DeleteEmployeeAsync(employeeId))
                                 .Return(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(employeeId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual(nameof(EmployeeController.Index), redirectResult.ActionName);
        }

      

    }

}
