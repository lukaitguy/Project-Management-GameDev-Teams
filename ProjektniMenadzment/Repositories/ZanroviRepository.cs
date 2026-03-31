using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Repositories
{
    public class ZanroviRepository : IZanroviRepository
    {
        private readonly PMDbContext _context;

        public ZanroviRepository(PMDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Zanrovi>> GetAllAsync()
        {
            return await _context.Zanrovis
                .OrderBy(z => z.Naziv)
                .ToListAsync();
        }
    }
}
