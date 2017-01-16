using Microsoft.AspNetCore.Mvc;

namespace AbcBank.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Type(int type)
        {
            if (type == 404)
            {
                return View("AccessDenied");
            }
            return View("AccessDenied");
        }
    }
}