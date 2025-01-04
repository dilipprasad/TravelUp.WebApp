using TravelUp.EmployeeAPI.Models;

namespace TravelUp.EmployeeAPI.Client.Interfaces
{
    public interface IEmployeeApiService
    {
        Task CreateEmployeeAsync(Employee newEmployee);
        Task DeleteEmployeeAsync(Guid id);
        Task<Employee> GetEmployeeByIdAsync(Guid id);
        Task<PaginatedResponse<Employee>> GetEmployeesAsync(int pageNumber, int pageSize);
        Task UpdateEmployeeAsync(Guid id, Employee updatedEmployee);

        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    }
}