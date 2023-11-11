using Group5_MusicPlayer.Data;
using Group5_MusicPlayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Group5_MusicPlayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly MusicPlayerDbContext _context;

        public HomeController(MusicPlayerDbContext context)
        {
 
            _context = context;
        }

        public IActionResult Index()
        {
            var musicPlayerDbContext = _context.Songs.Include(s => s.Author).Include(s => s.Category).Where(s => s.IsPrivate == false);
            return View(musicPlayerDbContext.ToList());
        }
        public IActionResult PlaySong(int? songId)
        {
            return RedirectToAction("Details", "Songs", new {id = songId });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //public 
    }
}