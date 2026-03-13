using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Data;

namespace ProjektniMenadzment.Repositories
{
    public class BuildoviRepository : IBuildoviRepository
    {
        private readonly PMDbContext _context;

        public BuildoviRepository(PMDbContext context)
        {
            _context = context;
        }

        public async Task<Buildovi> CreateAsync(Buildovi build)
        {
            await _context.Buildovis.AddAsync(build);
            await _context.SaveChangesAsync();
            return build;
        }

        public async Task<Buildovi?> DeleteAsync(Guid id)
        {
            var build = await _context.Buildovis.FirstOrDefaultAsync(b => b.Id == id);

            if (build == null)
            {
                return null;
            }

            _context.Buildovis.Remove(build);
            await _context.SaveChangesAsync();

            return build;
        }

        public async Task<List<Buildovi>> GetAllAsync()
        {
            return await _context.Buildovis
                .Include(b => b.Projekat)
                .OrderByDescending(b => b.DatumBuilda)
                .ToListAsync();
        }

        public async Task<Buildovi?> GetByIdAsync(Guid id)
        {
            return await _context.Buildovis
                .Include (b => b.Projekat)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Buildovi>> GetByProjekatIdAsync(Guid projekatId)
        {
            return await _context.Buildovis
                .Where(b => b.ProjekatId == projekatId)
                .OrderByDescending(b => b.DatumBuilda)
                .ToListAsync();    
        }

        public async Task<Buildovi?> UpdateAsync(Buildovi build)
        {
            var postojeciBuild = await _context.Buildovis.FirstOrDefaultAsync(b => b.Id == build.Id);

            if (postojeciBuild == null)
            {
                return null;
            }

            postojeciBuild.Verzija = build.Verzija;
            postojeciBuild.NazivBuilda = build.NazivBuilda;
            postojeciBuild.TipBuilda = build.TipBuilda;
            postojeciBuild.PatchNapomene = build.PatchNapomene;
            postojeciBuild.DatumBuilda = build.DatumBuilda;
            postojeciBuild.ProjekatId = build.ProjekatId;

            await _context.SaveChangesAsync();

            return postojeciBuild;
        }
    }
}
