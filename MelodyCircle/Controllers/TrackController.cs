using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MelodyCircle.Data;

namespace MelodyCircle.Controllers
{
    public class TrackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista todas as faixas de uma colaboração específica
        public async Task<IActionResult> Index(Guid collaborationId)
        {
            var tracks = await _context.Tracks
                            .Where(t => t.Collaboration.Id == collaborationId)
                            .Include(t => t.Instruments)
                            .ToListAsync();

            return View(tracks);
        }

        // Detalhes de uma faixa específica
        public async Task<IActionResult> Details(Guid id)
        {
            var track = await _context.Tracks
                            .Include(t => t.Instruments)
                            .FirstOrDefaultAsync(m => m.Id == id);

            if (track == null)
                return NotFound();

            return View(track);
        }

    }
}