using MelodyCircle.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MelodyCircle.ViewComponents
{
    public class MusicListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(UserMusicsViewModel favoriteMusicList)
        {
            return View(favoriteMusicList);
        }
    }
}
