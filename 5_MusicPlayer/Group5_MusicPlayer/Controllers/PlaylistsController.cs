using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group5_MusicPlayer.Data;
using Group5_MusicPlayer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;

namespace Group5_MusicPlayer.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly MusicPlayerDbContext _context;
        private readonly IHubContext<SignalrServer> _signalRHub;

        public PlaylistsController(MusicPlayerDbContext context, IHubContext<SignalrServer> signalRHub)
        {
            _context = context;
            _signalRHub = signalRHub;
        }

        // GET: Playlists
        public async Task<IActionResult> Index()
        {
            var account = HttpContext.Session.GetString("Account");
            if (account != "Admin")
                return RedirectToAction("Index", "Home");

            var musicPlayerDbContext = _context.Playlists.Include(p => p.User);
            return View(await musicPlayerDbContext.ToListAsync());
        }
        public async Task<IActionResult> SetPlayListIdSession(int userid)
        {
            if (userid == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .Include(p => p.User)
                .Where(p => p.User.UserId == userid)
                .FirstOrDefaultAsync();
            if (playlist == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("PlayListId", playlist.PlaylistId.ToString());
            return RedirectToAction("Index", "Home");
        }

        // GET: Playlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlaylistId == id);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }
        public async Task<IActionResult> UserPlaylistDetails(int userid)
        {
            if (userid == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .Include(p => p.User)
                .Where(p => p.User.UserId == userid)
                .FirstOrDefaultAsync();
            if (playlist == null)
            {
                return NotFound();
            }           
            return RedirectToAction("Details", "Playlists", new { id = playlist.PlaylistId });
        }

        // GET: Playlists/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlaylistId,Title,UserId")] Playlist playlist)
        {
                _context.Add(playlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }
        // GET: Playlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", playlist.UserId);
            return View(playlist);
        }

        // POST: Playlists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("PlaylistId,Title,UserId")] Playlist playlist)
        {
            //if (id != playlist.PlaylistId)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(playlist.PlaylistId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", playlist.UserId);
            return View(playlist);
        }

        // GET: Playlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlaylistId == id);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Playlists == null)
            {
                return Problem("Entity set 'MusicPlayerDbContext.Playlists'  is null.");
            }
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaylistExists(int id)
        {
          return (_context.Playlists?.Any(e => e.PlaylistId == id)).GetValueOrDefault();
        }

        public IActionResult CreateSongList()
        {
            return RedirectToAction("Create", "SongLists");
        }
    }
}
