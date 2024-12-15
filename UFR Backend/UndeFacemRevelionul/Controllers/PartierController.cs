using Microsoft.AspNetCore.Mvc;

namespace UndeFacemRevelionul.Controllers
{
    public class PartierController : Controller
    {
        public IActionResult Dashboard()
        {
            return View(); // Returnează pagina Dashboard.cshtml din Views/Partier/
        }
    }
}
