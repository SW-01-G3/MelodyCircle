using Microsoft.AspNetCore.Mvc;

namespace MelodyCircle.ViewComponents
{
    public class MusicListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<string> favoriteMusicList)
        {
            return View(favoriteMusicList);
        }
    }
}
