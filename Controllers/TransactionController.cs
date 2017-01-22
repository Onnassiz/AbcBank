using AbcBank.Models;
using Microsoft.AspNetCore.Mvc;

namespace AbcBank.Controllers
{
    public class TransactionController : Controller
    {
        private readonly MyDbContext _context;

        public TransactionController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}