using AutoMapper;
using TravelUp.EmployeeAPI.Models;

namespace TravelUp.WebApp.Mapping
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TravelUp.EmployeeAPI.Models.Employee, TravelUp.WebApp.Models.Employee>().ReverseMap();
            //CreateMap<TravelUp.EmployeeAPI.Models.PaginatedResponse<Employee>, TravelUp.WebApp.Models.PaginatedResponse<Employee>>();
            // Mapping for PaginatedResponse<T>
            CreateMap(typeof(TravelUp.EmployeeAPI.Models.PaginatedResponse<>), typeof(TravelUp.WebApp.Models.PaginatedResponse<>)).ReverseMap();
        }

    }
}
