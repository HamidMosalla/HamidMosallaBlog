using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VS2017Test.Controllers
{
    public class HomeSlider : ViewComponent
    {

        public IViewComponentResult Invoke(int id)
        {
            return View(id);
        }

    }
}
