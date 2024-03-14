using Microsoft.AspNetCore.Mvc;

namespace MelodyCircle.ViewComponents
{
    public class StarRatingViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(double rating)
        {
            return View(rating);
        }
    }
}
