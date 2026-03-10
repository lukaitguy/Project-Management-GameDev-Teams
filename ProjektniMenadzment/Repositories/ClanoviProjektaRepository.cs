using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Repositories
{
    public class ClanoviProjektaRepository : IClanoviProjektaRepository
    {
        private readonly PMDbContext _context;
        public ClanoviProjektaRepository(PMDbContext context)
        {
            _context = context;
        }

        public async Task<ClanoviProjektum> AddAsync(ClanoviProjektum clan)
        {
            await _context.ClanoviProjekta.AddAsync(clan);
            await _context.SaveChangesAsync();
            return clan;
        }
        public async Task<List<ClanoviProjektum>> GetByProjekatIdAsync(Guid projekatId)
        {
            return await _context.ClanoviProjekta
                .Include(cp => cp.Korisnik)
                .Where(cp => cp.ProjekatId == projekatId)
                .ToListAsync();
        }
        public async Task<List<Korisnici>> GetKorisniciByProjekatIdAsync(Guid projekatId)
        {
            return await _context.ClanoviProjekta
                .Include(cp => cp.Korisnik)
                .Where(cp => cp.ProjekatId == projekatId && cp.Korisnik != null)
                .Select(cp => cp.Korisnik)
                .ToListAsync();
        }


    }
}
