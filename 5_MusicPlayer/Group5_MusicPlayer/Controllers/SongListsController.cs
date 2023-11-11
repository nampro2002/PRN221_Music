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

namespace Group5_MusicPlayer.Controllers
{
    public class SongListsController : Controller
    {
        private readonly MusicPlayerDbContext _context;
        private readonly IHubContext<SignalrServer> _signalRHub;

        public SongListsController(MusicPlayerDbContext context, IHubContext<SignalrServer> signalRHub)
        {
            _context = context;
            _signalRHub = signalRHub;
        }

        // GET: SongLists
        public async Task<IActionResult> Index()
        {
            //var account = HttpContext.Session.GetString("Account");
            //if (account != "Admin")
            //    return RedirectToAction("Index", "Home");
            int playlistId = int.Parse(HttpContext.Session.GetString("PlayListId"));
            var musicPlayerDbContext = _context.SongsList.Include(s => s.Playlist).Include(s => s.Song);
            return View(await musicPlayerDbContext.ToListAsync());        
        }


        // GET: SongLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SongsList == null)
            {
                return NotFound();
            }

            var songList = await _context.SongsList
                .Include(s => s.Playlist)
                .Include(s => s.Song)
                .FirstOrDefaultAsync(m => m.SongListId == id);
            if (songList == null)
            {
                return NotFound();
            }

            return View(songList);
        }

        // GET: SongLists/Create
        public IActionResult Create()
        {
            //ViewData["PlaylistId"] = new SelectList(_context.Playlists, "PlaylistId", "PlaylistId");
            ViewData["SongId"] = new SelectList(_context.Songs, "SongId", "SongId");
            return View();
        }

        // POST: SongLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongListId,AddedDate,PlaylistId,SongId")] SongList songList)
        {
            int plId = int.Parse(HttpContext.Session.GetString("PlayListId"));
            songList.PlaylistId = plId;
            songList.AddedDate = DateTime.Now;
            _context.Add(songList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Add(int songid)
        {
            SongList songList = new SongList();
            int plId = int.Parse(HttpContext.Session.GetString("PlayListId"));
            songList.PlaylistId = plId;
            songList.AddedDate = DateTime.Now;
            songList.SongId = songid;
            _context.Add(songList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: SongLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SongsList == null)
            {
                return NotFound();
            }

            var songList = await _context.SongsList.FindAsync(id);
            if (songList == null)
            {
                return NotFound();
            }
            ViewData["PlaylistId"] = new SelectList(_context.Playlists, "PlaylistId", "PlaylistId", songList.PlaylistId);
            ViewData["SongId"] = new SelectList(_context.Songs, "SongId", "SongId", songList.SongId);
            return View(songList);
        }

        // POST: SongLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("SongListId,AddedDate,PlaylistId,SongId")] SongList songList)
        {
            //if (id != songList.SongListId)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(songList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongListExists(songList.SongListId))
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
            ViewData["PlaylistId"] = new SelectList(_context.Playlists, "PlaylistId", "PlaylistId", songList.PlaylistId);
            ViewData["SongId"] = new SelectList(_context.Songs, "SongId", "SongId", songList.SongId);
            return View(songList);
        }

        // GET: SongLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SongsList == null)
            {
                return NotFound();
            }

            var songList = await _context.SongsList
                .Include(s => s.Playlist)
                .Include(s => s.Song)
                .FirstOrDefaultAsync(m => m.SongListId == id);
            if (songList == null)
            {
                return NotFound();
            }

            return View(songList);
        }

        // POST: SongLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SongsList == null)
            {
                return Problem("Entity set 'MusicPlayerDbContext.SongsList'  is null.");
            }
            var songList = await _context.SongsList.FindAsync(id);
            if (songList != null)
            {
                _context.SongsList.Remove(songList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongListExists(int id)
        {
          return (_context.SongsList?.Any(e => e.SongListId == id)).GetValueOrDefault();
        }
    }
}
