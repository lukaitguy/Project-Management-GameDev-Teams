using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;
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
            var postojeciResurs = await _context.Resursis.FindAsync(id);
            if (postojeciResurs == null)
            {
                return false;
            }
            _context.Resursis.Remove(postojeciResurs);
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

        public async Task<Resursi> GetByIdAsync(Guid id)
        {
            var postojeciResurs = await _context.Resursis
                                    .Include(r => r.Projekat)
                                    .Include(r => r.DodeljenKorisnikuNavigation)
                                    .FirstOrDefaultAsync(r => r.Id == id);
            if (postojeciResurs != null)
            {
                return postojeciResurs;
            }
            else
            {
                throw new KeyNotFoundException($"Resurs sa ID {id} nije pronađen.");
            }
        }

        public async Task<IEnumerable<Resursi>> GetByProjekatIdAsync(Guid projekatId)
        {
            var resursi = await _context.Resursis
                .Where(r => r.ProjekatId == projekatId)
                .Include(r => r.Projekat)
                .Include(r => r.DodeljenKorisnikuNavigation)
                .ToListAsync();

            if (resursi != null)
            {
                return resursi;
            }
            else
            {
                throw new KeyNotFoundException($"Nema resursa za projekat sa ID {projekatId}.");
            }
        }

        public async Task<Resursi> UpdateAsync(Resursi resurs)
        {
            var postojeciResurs = await _context.Resursis.FindAsync(resurs.Id);

            if (postojeciResurs != null)
            {
                postojeciResurs.Naziv = resurs.Naziv;
                postojeciResurs.Opis = resurs.Opis;
                postojeciResurs.Tip = resurs.Tip;
                postojeciResurs.Cena = resurs.Cena;
                postojeciResurs.ProjekatId = resurs.ProjekatId;
                postojeciResurs.DodeljenKorisniku = resurs.DodeljenKorisniku;

                _context.Resursis.Update(postojeciResurs);
                await _context.SaveChangesAsync();
                return postojeciResurs;
            }
            else
            {
                throw new KeyNotFoundException($"Resurs sa ID {resurs.Id} nije pronadjen.");
            }

        }
    }
}
