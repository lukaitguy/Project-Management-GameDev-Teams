using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Repositories
{
    public class ResursiRepository : IResursiRepository
    {
        private readonly PMDbContext _context;

        public ResursiRepository(PMDbContext context)
        {
            _context = context;
        }

        public async Task<Resursi> AddAsync(Resursi resurs)
        {
            await _context.Resursis.AddAsync(resurs);
            await _context.SaveChangesAsync();
            return resurs;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var resurs = await _context.Resursis.FindAsync(id);
            if (resurs == null) return false;
            _context.Resursis.Remove(resurs);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Resursi>> GetAllAsync()
        {
            return await _context.Resursis
                .Include(r => r.Projekat)
                .Include(r => r.DodeljenKorisnikuNavigation)
                .ToListAsync();
        }

        public async Task<Resursi?> GetByIdAsync(Guid id)
        {
            return await _context.Resursis
                .Include(r => r.Projekat)
                .Include(r => r.DodeljenKorisnikuNavigation)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Resursi>> GetByProjekatIdAsync(Guid projekatId)
        {
            return await _context.Resursis
                .Where(r => r.ProjekatId == projekatId)
                .Include(r => r.DodeljenKorisnikuNavigation)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resursi>> GetByKorisnikIdAsync(Guid korisnikId)
        {
            return await _context.Resursis
                .Include(r => r.DodeljenKorisnikuNavigation)
                .Include(r => r.Projekat)
                .Where(r => r.DodeljenKorisniku == korisnikId)
                .OrderByDescending(r => r.DatumKreiranja)
                .ToListAsync();
        }

        public async Task<Resursi?> UpdateAsync(Resursi resurs)
        {
            var postojeci = await _context.Resursis.FindAsync(resurs.Id);
            if (postojeci == null) return null;

            postojeci.Naziv = resurs.Naziv;
            postojeci.Opis = resurs.Opis;
            postojeci.Tip = resurs.Tip;
            postojeci.Cena = resurs.Cena;
            postojeci.ProjekatId = resurs.ProjekatId;
            postojeci.DodeljenKorisniku = resurs.DodeljenKorisniku;

            await _context.SaveChangesAsync();
            return postojeci;
        }
    }
}