
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group5_MusicPlayer.Data;
using Group5_MusicPlayer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;

namespace Group5_MusicPlayer.Controllers
{
    public class SongsController : Controller
    {
        private readonly MusicPlayerDbContext _context;
        private readonly IHubContext<SignalrServer> _signalRHub;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        public SongsController(MusicPlayerDbContext context, IHubContext<SignalrServer> signalRHub, Microsoft.AspNetCore.Hosting.IHostingEnvironment _Environment)
        {
            _context = context;
            _signalRHub = signalRHub;
            Environment = _Environment;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(HttpContext.Session.GetString("ID"));
            if (userId != null && userId == 1)
            {
               var  musicPlayerAdmin = _context.Songs.Include(s => s.Author).Include(s => s.Category);
                return View(await musicPlayerAdmin.ToListAsync());
            }
            else if (userId != null && userId != 1)
            {
                var musicPlayerUser = _context.Songs.Include(s => s.Author).Include(s => s.Category).Where(s=>s.AuthorId == userId);
                return View(await musicPlayerUser.ToListAsync());
            }
           return RedirectToAction("Index", "Home");
           
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Author)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        public IActionResult UploadFile(List<IFormFile> postedFiles)
        {
            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            string path = Path.Combine(this.Environment.WebRootPath, "Upload");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            List<string> uploadFiles = new List<string>();
            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = Path.GetFileName(postedFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    uploadFiles.Add(fileName);
                }
                Console.WriteLine("fileName:",fileName, path);
            }            
            return RedirectToPage("/Create"); ;
        }
        // GET: Songs/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongId,Title,CategoryId,AuthorId,ImgPath,AudioPath,IsPrivate")] Song song, List<IFormFile> postedFiles)
        {
            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            string path = Path.Combine(this.Environment.WebRootPath, "Upload");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            List<string> uploadFiles = new List<string>();
            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = Path.GetFileName(postedFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    uploadFiles.Add(fileName);
                }
                Console.WriteLine("fileName:", fileName, path);
                song.AudioPath = fileName;
            }
            if (song.AuthorId == 0)
            {
                int userId = int.Parse(HttpContext.Session.GetString("ID"));
                song.AuthorId = userId;
            }
                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "UserId", song.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", song.CategoryId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("SongId,Title,CategoryId,AuthorId,ImgPath,AudioPath,IsPrivate")] Song song, List<IFormFile> postedFiles)
        {

            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            string path = Path.Combine(this.Environment.WebRootPath, "Upload");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            List<string> uploadFiles = new List<string>();
            foreach (IFormFile postedFile in postedFiles)
            {
                string fileName = Path.GetFileName(postedFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    uploadFiles.Add(fileName);
                }
                Console.WriteLine("fileName:", fileName, path);
                song.AudioPath = fileName;
            }
            if (song.AuthorId == 0)
            {
                int userId = int.Parse(HttpContext.Session.GetString("ID"));
                song.AuthorId = userId;
            }
            _context.Update(song);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));           
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Author)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Songs == null)
            {
                return Problem("Entity set 'MusicPlayerDbContext.Songs'  is null.");
            }
            Song song = await _context.Songs.Include(s => s.SongLists).FirstOrDefaultAsync(s => s.SongId == id);

            if (song != null)
            {
                if (song.SongLists.Count > 0)
                {
                    foreach (SongList songList in song.SongLists)
                    {
                        songList.SongId = 11;
                    }
                }
                _context.Songs.Remove(song);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
          return (_context.Songs?.Any(e => e.SongId == id)).GetValueOrDefault();
        }
    }
}
