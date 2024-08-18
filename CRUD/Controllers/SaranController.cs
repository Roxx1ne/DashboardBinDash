using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRUD.Models;
using CRUD.Context;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CRUD.Controllers
{
    [Authorize(Roles = "Member")]
    public class SaranController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaranController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Form Saran untuk User
        public IActionResult Index()
        {
            return View(_context.SaranModel.ToList());
        }

        // GET: Create Saran (User)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kirim Saran (User)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaranModel saran)
        {
            if (ModelState.IsValid)
            {
                saran.TanggalDikirim = DateTime.Now;
                _context.Add(saran);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(saran);
        }

        // GET: Edit Saran (User)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saran = await _context.SaranModel.FindAsync(id);
            if (saran == null)
            {
                return NotFound();
            }
            return View(saran);
        }

        // POST: Edit Saran (User)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SaranModel saran)
        {
            if (id != saran.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(saran);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaranExists(saran.Id))
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
            return View(saran);
        }

        // GET: Delete Saran (User)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saran = await _context.SaranModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saran == null)
            {
                return NotFound();
            }

            return View(saran);
        }

        // POST: Delete Saran Confirmed (User)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saran = await _context.SaranModel.FindAsync(id);
            _context.SaranModel.Remove(saran);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaranExists(int id)
        {
            return _context.SaranModel.Any(e => e.Id == id);
        }
    }
}