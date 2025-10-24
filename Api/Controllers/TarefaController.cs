using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class TarefaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
