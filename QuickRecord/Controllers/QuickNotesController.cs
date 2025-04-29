using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickRecord.Data;
using QuickRecord.Models;

namespace QuickRecord.Controllers
{
    public class QuickNotesController : Controller
    {
        private readonly AppDbContext _ctx;
        public QuickNotesController(AppDbContext ctx) => _ctx = ctx;

        // GET /
        public async Task<IActionResult> Index(string? search)
        {
            var query = _ctx.QuickNotes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(q => q.Content.Contains(search));
                ViewData["Search"] = search;
            }

            // 以時間倒序排列
            var notes = await query.OrderByDescending(q => q.CreatedUtc).ToListAsync();
            return View(notes);
        }

        // POST /QuickNotes/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string content)
        {
            if (content.Length > 0 && content.Length <= 500)
            {
                _ctx.Add(new QuickNote { Content = content });
                await _ctx.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
