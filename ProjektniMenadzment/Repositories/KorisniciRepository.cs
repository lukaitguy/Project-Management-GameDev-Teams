using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Repositories
{
    public class KorisniciRepository : IKorisniciRepository
    {
        private readonly PMDbContext _context;
        public KorisniciRepository(PMDbContext context)
        {
            _context = context;
        }

        public async Task<List<Korisnici>> GetAllAsync()
        {
            return await _context.Korisnicis.ToListAsync();
        }

        public async Task<Korisnici> GetByIdAsync(Guid id)
        {
            return await _context.Korisnicis
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<List<SelectListItem>> GetSelectOptionsAsync()
        {
            return await _context.Korisnicis
                .Select(k => new SelectListItem
                {
                    Value = k.Id.ToString(),
                    Text = $"{k.Ime} {k.Prezime}"
                })
                .ToListAsync();
        }

        public async Task<Korisnici> AddAsync(Korisnici korisnik)
        {
            await _context.Korisnicis.AddAsync(korisnik);
            await _context.SaveChangesAsync();
            return korisnik;
        }

        public async Task<Korisnici?> GetByIdentityUserIdAsync(string identityUserId)
        {
            return await _context.Korisnicis
                .FirstOrDefaultAsync(k => k.IdentityUserId == identityUserId);
        }

        public async Task<Korisnici?> GetByEmailAsync(string email)
        {
            return await _context.Korisnicis
                .FirstOrDefaultAsync(k => k.Email == email);
        }
    }
}
