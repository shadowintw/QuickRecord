using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;             // ★ 取 OID 的擴充方法
using QuickRecord.Data;
using QuickRecord.Models;

namespace QuickRecord.Controllers
{
    [Authorize]                            // 任何登入者都需驗證
    public class QuickNotesController : Controller
    {
        private readonly AppDbContext _ctx;
        public QuickNotesController(AppDbContext ctx) => _ctx = ctx;

        // 目前使用者的 Object Id（若未登入則為 null）
        private string? CurrentUserOid => User.GetObjectId();

        // GET: /QuickNotes
        public async Task<IActionResult> Index(string? search)
        {
            IQueryable<QuickNote> query = _ctx.QuickNotes;

            // 非 Admin 僅能查看自己的資料
            if (!User.IsInRole("Admin"))
            {
                query = query.Where(q => q.OwnerId == CurrentUserOid);
            }

            // 關鍵字搜尋
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(q => q.Content.Contains(search));
                ViewData["Search"] = search;
            }

            var notes = await query
                .OrderByDescending(q => q.CreatedUtc)
                .ToListAsync();

            return View(notes);
        }

        // POST: /QuickNotes/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string content)
        {
            // 未登入則導向登入流程
            if (!User.Identity?.IsAuthenticated ?? true)
                return Challenge();

            // 保險起見再次確認 OID
            var oid = CurrentUserOid;
            if (oid is null) return Forbid();

            if (string.IsNullOrWhiteSpace(content) || content.Length > 500)
                return RedirectToAction(nameof(Index));

            var note = new QuickNote
            {
                Content = content,
                CreatedUtc = DateTime.UtcNow,
                OwnerId = oid
            };

            _ctx.Add(note);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /QuickNotes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _ctx.QuickNotes.FindAsync(id);
            if (note == null) return NotFound();

            if (!User.IsInRole("Admin") && note.OwnerId != CurrentUserOid)
                return Forbid();

            return View(note);
        }

        // POST: /QuickNotes/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content")] QuickNote input)
        {
            if (id != input.Id) return BadRequest();

            var note = await _ctx.QuickNotes.FindAsync(id);
            if (note == null) return NotFound();

            if (!User.IsInRole("Admin") && note.OwnerId != CurrentUserOid)
                return Forbid();

            if (string.IsNullOrWhiteSpace(input.Content) || input.Content.Length > 500)
                return View(note);

            note.Content = input.Content;
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /QuickNotes/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _ctx.QuickNotes.FindAsync(id);
            if (note == null) return NotFound();

            if (!User.IsInRole("Admin") && note.OwnerId != CurrentUserOid)
                return Forbid();

            _ctx.QuickNotes.Remove(note);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
