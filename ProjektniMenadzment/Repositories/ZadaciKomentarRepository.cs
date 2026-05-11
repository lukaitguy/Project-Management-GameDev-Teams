using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Repositories
{
    public class ZadaciKomentarRepository : IZadaciKomentarRepository
    {
        private readonly PMDbContext _context;

        public ZadaciKomentarRepository(PMDbContext context)
        {
            _context = context;
        }

        public async Task<KomentariZadatak> AddAsync(KomentariZadatak komentar)
        {
            await _context.KomentariZadataks.AddAsync(komentar);
            await _context.SaveChangesAsync();
            return komentar;
        }

        public async Task<IEnumerable<KomentariZadatak>> GetAllByIdAsync(Guid zadatakId)
        {
            return await _context.KomentariZadataks
                .Include(k => k.Korisnik)
                .Where(k => k.ZadatakId == zadatakId)
                .OrderBy(k => k.DatumKreiranja)
                .ToListAsync();
        }
    }
}
