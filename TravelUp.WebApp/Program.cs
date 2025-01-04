using AutoMapper;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TravelUp.EmployeeAPI.Client;
using TravelUp.EmployeeAPI.Client.Interfaces;
using TravelUp.WebApp.Mapping;

namespace TravelUp.WebApp
{
    public class Program
    {
        public static  void Main(string[] args)
        {
            string environmentName = string.Empty;
            try
            {
                environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            }
            catch (Exception ex)
            {

                Console.WriteLine("Problem Fetching Environment Variables",ex);
            }

            var builder = WebApplication.CreateBuilder(args);
            IConfigurationRoot? config;
#if DEBUG
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
#else
            config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile($"appsettings.{environmentName}.json")
                            .Build();
#endif
            ConfigureServices(builder.Services,config);

            var app = builder.Build();



            //Health check
            //var services = app.Services.GetRequiredService<IEnumerable<IHostedService>>();



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.MapHealthChecks("/healthz");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

           
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services,IConfiguration config)
        {
            services.AddHealthChecks().AddResourceUtilizationHealthCheck();


            var employeeAPIURL = config.GetConnectionString("ServiceEndpoint");
            if (string.IsNullOrWhiteSpace(employeeAPIURL))
            {
                throw new Exception("Unable to find the Service Endpoint for Employee API");
            }
            // Register HttpClient and other services
            services.AddHttpClient<IEmployeeApiService, EmployeeApiService>("EmployeeAPI", (serviceProvider, client) =>
            {
                client.BaseAddress = new Uri(employeeAPIURL);
            });

            //services.AddScoped<IEmployeeApiService, EmployeeApiService>();

            //Add Mapping Configuration
            var mapperConfg = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
            var mapper = new Mapper(mapperConfg);
            services.AddSingleton<IMapper>(mapper);


            // Add services to the container.
            services.AddControllersWithViews();

            services.AddHealthChecks();

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "RequestVerificationToken";
            });

        }



    }
}
