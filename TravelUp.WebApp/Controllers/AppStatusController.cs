using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelUp.WebApp.Controllers
{
    [AllowAnonymous]
    public class AppStatusController : Controller
    {

        [HttpGet]
        [Route("Ping")]
        public ActionResult Ping()
        {
            ActionResult result = null;
            try
            {
                result = Content(string.Format("Request Received at {0:MM/dd/yyyy hh:mm:ss tt}", DateTime.Now));
            }
            catch (Exception ex)
            {

                result = Content("Problem Pinging the app");
            }
            return result;
        }
       
    }
}
