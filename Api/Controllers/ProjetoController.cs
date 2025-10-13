using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ProjetoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
