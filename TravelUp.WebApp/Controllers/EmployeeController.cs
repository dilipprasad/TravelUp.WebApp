using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TravelUp.EmployeeAPI.Client.Interfaces;
using TravelUp.WebApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TravelUp.WebApp.Controllers
{
    public class EmployeeController : BaseController
    {
        #region PrivateProperties
        private readonly IEmployeeApiService _employeeProvider;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeController> _logger;
        #endregion PrivateProperties

        #region PrivateMethods
        private IActionResult HandleEmployeeNotFound(Guid id)
        {
            return NotFound(new
            {
                Success = false,
                Message = "Employee not found",
                EmployeeId = id
            });
        }
        private EmployeeAPI.Models.Employee GetMappedEmployeeForAPI(Employee employee)
        {
            return _mapper.Map<TravelUp.EmployeeAPI.Models.Employee>(employee);
        }

        #endregion
        public EmployeeController(IEmployeeApiService employeeProvider, IMapper mapper, ILogger<EmployeeController> logger)
            : base(logger)
        {
            _employeeProvider = employeeProvider;
            _mapper = mapper;
            _logger = logger;
        }

       

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Index page.");
                return HandleExceptionResponse(ex, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesPartial(int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                var employees = await _employeeProvider.GetEmployeesAsync(pageNumber, pageSize);
                var mappedEmp = _mapper.Map<PaginatedResponse<Employee>>(employees);
                return PartialView("_EmployeeTablePartial", mappedEmp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employees partial view.");
                return HandleExceptionResponse(ex, "An error occurred while fetching employees.");
            }
        }

        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Create page.");
                return HandleExceptionResponse(ex, "An error occurred while processing your request.");

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Invalid model state" });
                }

                var mappedEmp = GetMappedEmployeeForAPI(employee);
                await _employeeProvider.CreateEmployeeAsync(mappedEmp);

                return Json(new { success = true, message = "Employee created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee.");
                return Json(new { success = false, message = "An error occurred while creating the employee." });
            }
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var employee = await _employeeProvider.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return HandleEmployeeNotFound(id);
                }

                var mappedEmp = _mapper.Map<TravelUp.WebApp.Models.Employee>(employee);
                return View(mappedEmp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error editing employee with ID {id}.");

                return HandleExceptionResponse(ex, "An error occurred while fetching the employee details.");

            }
        }

      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] Employee employee)
        {
            try
            {
                if (employee == null)
                {
                    return HandleBadRequest("Employee data is null or invalid.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Errors = ModelState.SelectMany(x => x.Value.Errors)
                                           .Select(e => e.ErrorMessage)
                    });
                }

                if (!employee.Id.HasValue)
                {
                    return HandleBadRequest("Employee ID is required.");
                }

                var mappedEmp = GetMappedEmployeeForAPI(employee);
                await _employeeProvider.UpdateEmployeeAsync(employee.Id.Value, mappedEmp);

                return HandleSuccessReponse("Employee updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating employee with ID {employee?.Id}.");
                return HandleExceptionResponse(ex, "An error occurred while updating the employee.");
            }
        }


        [HttpDelete()]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var employee = await _employeeProvider.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return HandleEmployeeNotFound(id);
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Employee fetched successfully.",
                    Data = employee
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching employee for deletion with ID {id}.");
                return HandleExceptionResponse(ex, "An error occurred while fetching the employee details.");
            }
        }

        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var employee = await _employeeProvider.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return HandleEmployeeNotFound(id);
                }

                await _employeeProvider.DeleteEmployeeAsync(id);

                return Ok(new
                {
                    Success = true,
                    Message = "Employee deleted successfully.",
                    EmployeeId = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID {id}.");
                return HandleExceptionResponse(ex, "An error occurred while deleting the employee.");
            }
        }
    }

}
