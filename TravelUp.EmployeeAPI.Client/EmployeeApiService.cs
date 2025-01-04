using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net;
using System.Net.Http.Json;
using TravelUp.EmployeeAPI.Client.Interfaces;
using TravelUp.EmployeeAPI.Models;

namespace TravelUp.EmployeeAPI.Client
{
  
    public class EmployeeApiService : IEmployeeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogger<IEmployeeApiService> _logger;


        public EmployeeApiService(HttpClient httpClient, IConfiguration? configuration,ILogger<IEmployeeApiService> logger)

        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            logger.LogInformation($"HttpClient BaseAddress: {_httpClient.BaseAddress}");

            // Load retry settings from the configuration
            var maxRetries = _configuration.GetValue<int>("RetrySettings:MaxRetries");
            var delaySeconds = _configuration.GetValue<int>("RetrySettings:DelaySeconds");


            // Define the retry policy: Retry a number of times with an exponential backoff delay
            _retryPolicy = Policy
                .Handle<HttpRequestException>()  // Retry only on specific exceptions, like HttpRequestException
                .WaitAndRetryAsync(maxRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(delaySeconds, retryAttempt))
                );

            // BaseAddress is typically set in Program.cs or here:
            // _httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
        }

        public async Task<PaginatedResponse<Employee>> GetEmployeesAsync(int pageNumber, int pageSize)
        {
            try
            {
                // Format the URL with query parameters
                var url = $"Employee/GetPaginated?pageNumber={pageNumber}&pageSize={pageSize}";

                // Send GET request and parse the response as PaginatedResponse<Employee>
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.GetFromJsonAsync<PaginatedResponse<Employee>>(url);
                    if (response == null)
                    {
                        throw new Exception("Failed to retrieve employees.");
                    }
                    return response;
                });
            }
            catch (HttpRequestException ex)
            {
                // Handle network-related issues or invalid responses
                throw new Exception("An error occurred while fetching the employees.", ex);
            }
            catch (Exception ex)
            {
                // Handle other errors (e.g., deserialization issues)
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                // We assume the endpoint is GET /employees
                // Adjust if your actual route is "employee" or "api/employee"
                var endpoint = "GetAllEmployees";

                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var response = await _httpClient.GetAsync(endpoint);

                    // Throw if not a success status code (e.g., 4xx or 5xx)
                    // This will let us handle HttpRequestException or custom logic
                    if (!response.IsSuccessStatusCode)
                    {
                        // For 500, you might want to handle differently
                        if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            throw new Exception("A server error occurred while retrieving employees (500).");
                        }

                        // Otherwise, throw a generic exception
                        response.EnsureSuccessStatusCode();
                    }

                    // Deserialize the response content to a collection of Employee
                    var employees = await response.Content.ReadFromJsonAsync<IEnumerable<Employee>>();

                    // If deserialization fails or returns null, throw an exception
                    if (employees is null)
                    {
                        throw new Exception("Failed to deserialize the employees list from the server.");
                    }

                    return employees;
                });
            }
            catch (HttpRequestException ex)
            {
                // Typically thrown for network-related issues (DNS, connection refused, etc.)
                throw new Exception("An HTTP request error occurred while fetching employees.", ex);
            }
            catch (Exception ex)
            {
                // Handle all other exceptions (deserialization, custom exceptions, etc.)
                throw new Exception("An unexpected error occurred while fetching employees.", ex);
            }
        }

        public async Task<Employee> GetEmployeeByIdAsync(Guid id)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.GetFromJsonAsync<Employee>($"Employee/{id}");
                    if (response == null)
                    {
                        throw new Exception($"Failed to retrieve employee with ID {id}.");
                    }
                    return response;
                });
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"An error occurred while fetching employee with ID {id}.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task CreateEmployeeAsync(Employee newEmployee)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.PostAsJsonAsync("Employee", newEmployee);
                    response.EnsureSuccessStatusCode();
                });
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("An error occurred while creating the employee.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task UpdateEmployeeAsync(Guid id, Employee updatedEmployee)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.PutAsJsonAsync($"Employee/{id}", updatedEmployee);
                    response.EnsureSuccessStatusCode();
                });
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"An error occurred while updating the employee with ID {id}.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task DeleteEmployeeAsync(Guid id)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var response = await _httpClient.DeleteAsync($"Employee/{id}");
                    response.EnsureSuccessStatusCode();
                });
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"An error occurred while deleting the employee with ID {id}.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

    }
}
