using Microsoft.AspNetCore.Mvc;

namespace TravelUp.WebApp.Controllers
{
    public class BaseController : Controller
    {
        private readonly ILogger<BaseController> _logger;

        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }
        protected IActionResult HandleExceptionResponse(Exception ex, string message)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = message,
                Details = ex.Message
            });
        }
        protected IActionResult HandleBadRequest(string message)
        {

            return BadRequest(new
            {
                Success = false,
                Message = message
            });
        }

        protected IActionResult HandleSuccessReponse(string message)
        {
            return Ok(new
            {
                Success = true,
                Message = message
            });
        }
        protected IActionResult HandleNotFoundReponse(string message)
        {
            return NotFound(new
            {
                Success = false,
                Message = message
            });
        }
       
    }
}
