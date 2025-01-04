using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using TravelUp.EmployeeAPI.Client.Interfaces;
using TravelUp.EmployeeAPI.Models;
using TravelUp.WebApp.Controllers;
using TravelUp.WebApp.Models;

namespace TravelUp.WebApp.Tests.Controllers
{

    [TestClass]
    public class EmployeeControllerTests
    {
        private IEmployeeApiService _employeeProviderMock;
        private AutoMapper.IMapper _mapperMock;
        private ILogger<TravelUp.WebApp.Controllers.EmployeeController> _loggerMock;
        private TravelUp.WebApp.Controllers.EmployeeController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _employeeProviderMock = MockRepository.GenerateMock<IEmployeeApiService>();
            _mapperMock = MockRepository.GenerateMock<AutoMapper.IMapper>();
            _loggerMock = MockRepository.GenerateMock<ILogger<TravelUp.WebApp.Controllers.EmployeeController>>();
            _controller = new TravelUp.WebApp.Controllers.EmployeeController(_employeeProviderMock, _mapperMock, _loggerMock);
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
        public async Task GetEmployeesPartial_ShouldReturnPartialViewResult_WithMappedEmployees()
        {
            // Arrange
            var employeesApiResponse = new TravelUp.EmployeeAPI.Models.PaginatedResponse<TravelUp.EmployeeAPI.Models.Employee>
            {
                Items = new List<TravelUp.EmployeeAPI.Models.Employee>
                {
                    new TravelUp.EmployeeAPI.Models.Employee { Id = Guid.NewGuid(), Name = "John Doe" }
                },
                TotalCount = 1
            };
            var mappedEmployees = new TravelUp.WebApp.Models.PaginatedResponse<TravelUp.WebApp.Models.Employee>
            {
                Items = new List<TravelUp.WebApp.Models.Employee>
                {
                    new TravelUp.WebApp.Models.Employee { Id = Guid.NewGuid(), Name = "John Doe" }
                },
                TotalCount = 1
            };

            _employeeProviderMock.Stub(p => p.GetEmployeesAsync(Arg<int>.Is.Anything, Arg<int>.Is.Anything))
                                 .Return(Task.FromResult(employeesApiResponse));
            _mapperMock.Stub(m => m.Map<TravelUp.WebApp.Models.PaginatedResponse<TravelUp.WebApp.Models.Employee>>(employeesApiResponse))
                       .Return(mappedEmployees);

            // Act
            var result = await _controller.GetEmployeesPartial();

            // Assert
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            var partialViewResult = result as PartialViewResult;
            Assert.AreEqual("_EmployeeTablePartial", partialViewResult.ViewName);
            Assert.AreEqual(mappedEmployees, partialViewResult.Model);
        }

      

        [TestMethod]
        public async Task Edit_Get_ShouldReturnViewResult_WithMappedEmployee()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var employeeApiModel = new TravelUp.EmployeeAPI.Models.Employee { Id = employeeId, Name = "Jane Doe" };
            var mappedEmployee = new TravelUp.WebApp.Models.Employee { Id = employeeId, Name = "Jane Doe" };

            _employeeProviderMock.Stub(p => p.GetEmployeeByIdAsync(employeeId))
                                 .Return(Task.FromResult(employeeApiModel));
            _mapperMock.Stub(m => m.Map<TravelUp.WebApp.Models.Employee>(employeeApiModel))
                       .Return(mappedEmployee);

            // Act
            var result = await _controller.Edit(employeeId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual(mappedEmployee, viewResult.Model);
        }

        [TestMethod]
        public async Task Edit_Get_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _employeeProviderMock.Stub(p => p.GetEmployeeByIdAsync(employeeId))
                                 .Return(Task.FromResult<TravelUp.EmployeeAPI.Models.Employee>(null));

            // Act
            var result = await _controller.Edit(employeeId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;

        }

        [TestMethod]
        public async Task Delete_Post_ShouldReturnOkResult_WhenSuccessful()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _employeeProviderMock.Stub(p => p.GetEmployeeByIdAsync(employeeId))
                                 .Return(Task.FromResult(new TravelUp.EmployeeAPI.Models.Employee { Id = employeeId }));
            _employeeProviderMock.Stub(p => p.DeleteEmployeeAsync(employeeId))
                                 .Return(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(employeeId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
           
        }

        [TestMethod]
        public async Task Delete_Post_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            _employeeProviderMock.Stub(p => p.GetEmployeeByIdAsync(employeeId))
                                 .Return(Task.FromResult<TravelUp.EmployeeAPI.Models.Employee>(null));

            // Act
            var result = await _controller.Delete(employeeId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
           
        }
    }
}


