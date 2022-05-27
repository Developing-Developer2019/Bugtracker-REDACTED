using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bugtracker.Areas.Bug.Models.Bug;
using Bugtracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Bugtracker.Areas.Bug.Controllers
{
    [Area("Bug")]
    public class BugController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public BugController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // GET: Bug/Bug
        public async Task<IActionResult> Index()
        {
            return _context.Bug != null ? 
                          View(await _context.Bug.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.BugBO'  is null.");
        }

        // GET: Bug/Bug/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bug == null)
            {
                return NotFound();
            }

            var bugBO = await _context.Bug
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bugBO == null)
            {
                return NotFound();
            }

            return View(bugBO);
        }

        // GET: Bug/Bug/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bug/Bug/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Bug_Name,Bug_Description,Bug_CreatedDateT,Bug_ModifiedDateT,Bug_Status,Bug_ModifiedBy,Id")] BugBO bugBO)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bugBO);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bugBO);
        }

        // GET: Bug/Bug/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bug == null)
            {
                return NotFound();
            }

            var bugBO = await _context.Bug.FindAsync(id);
            if (bugBO == null)
            {
                return NotFound();
            }
            return View(bugBO);
        }

        // POST: Bug/Bug/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Bug_Name,Bug_Description,Bug_CreatedDateT,Bug_ModifiedDateT,Bug_Status,Bug_ModifiedBy,Id")] BugBO bugBO)
        {
            if (id != bugBO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bugBO);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BugExists(bugBO.Id))
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
            return View(bugBO);
        }

        // GET: Bug/Bug/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bug == null)
            {
                return NotFound();
            }

            var bugBO = await _context.Bug
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bugBO == null)
            {
                return NotFound();
            }

            return View(bugBO);
        }

        // POST: Bug/Bug/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bug == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BugBO'  is null.");
            }
            var bugBO = await _context.Bug.FindAsync(id);
            if (bugBO != null)
            {
                _context.Bug.Remove(bugBO);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BugExists(int id)
        {
          return (_context.Bug?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
