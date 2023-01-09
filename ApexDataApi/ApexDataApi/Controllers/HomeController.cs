using Microsoft.AspNetCore.Mvc;

namespace ApexDataApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    //[Route("frontend/[controller]")]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [HttpGet("Home"), Route("index")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
