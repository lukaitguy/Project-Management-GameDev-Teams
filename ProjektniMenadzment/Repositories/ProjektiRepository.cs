using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Repositories
{
    public class ProjektiRepository : IProjektiRepository
    {
        private readonly PMDbContext _context;

        public ProjektiRepository(PMDbContext context)
        {
            _context = context;
        }

        public async Task<Projekti> GetByIdAsync(Guid id)
        {
            return await _context.Projektis
                .Include(p => p.KreiraoKorisnik)
                .Include(p => p.Zanr)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Projekti> AddAsync(Projekti projekat)
        {
            await _context.Projektis.AddAsync(projekat);
            await _context.SaveChangesAsync();
            return projekat;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var postojeciProjekat = await _context.Projektis.FindAsync(id);

            if(postojeciProjekat == null)
            {
                return false;
            }

            _context.Projektis.Remove(postojeciProjekat);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Projekti>> GetAllAsync()
        {
            var projekti = await _context.Projektis
                .Include(p=> p.KreiraoKorisnik)                
                .ToListAsync();

            return projekti;
        }

        public async Task<PregledProjektaViewModel?> GetDetailsByIdAsync(Guid projekatId)
        {
            var projekat = await _context.Projektis
                .Include(p => p.KreiraoKorisnik)
                .Include(p => p.Zanr)
                .FirstOrDefaultAsync(p => p.Id == projekatId);

            if (projekat == null)
                return null;

            var clanovi = await _context.ClanoviProjekta
                .Include(cp => cp.Korisnik)
                .Where(cp => cp.ProjekatId == projekatId)
                .ToListAsync();

            var zadaci = await _context.Zadacis
                .Include(z => z.DodeljenKorisniku)
                .Where(z => z.ProjekatId == projekatId)
                .ToListAsync();

            var resursi = await _context.Resursis
                .Where(r => r.ProjekatId == projekatId)
                .ToListAsync();

            return new PregledProjektaViewModel
            {
                Id = projekat.Id,
                Naziv = projekat.Naziv,
                Opis = projekat.Opis ?? "Ovaj projekat nema opis.",
                Zanr = projekat.Zanr?.Naziv ?? "Ovaj projekat nema definisan zanr igre.",
                Status = projekat.Status,
                Budzet = projekat.Budzet ?? 0,
                DatumPocetka = projekat.DatumPocetka,
                Rok = projekat.Rok,
                KreiraoKorisnikIme = $"{projekat.KreiraoKorisnik.Ime} {projekat.KreiraoKorisnik.Prezime}",

                Clanovi = clanovi.Select(cp => new ClanTimaViewModel
                {
                    Ime = cp.Korisnik.Ime,
                    Prezime = cp.Korisnik.Prezime,
                    Uloga = cp.Uloga
                }).ToList(),

                Zadaci = zadaci.Select(z => new ZadaciViewModel
                {
                    Id = z.Id,
                    Naslov = z.Naslov,
                    Status = z.Status,
                    Prioritet = z.Prioritet,
                    Rok = z.Rok,
                    DodeljenKorisnikuIme = z.DodeljenKorisniku != null
                        ? $"{z.DodeljenKorisniku.Ime} {z.DodeljenKorisniku.Prezime}"
                        : "Nije dodeljen"
                }).ToList(),

                Resursi = resursi.Select(r => new ResursiViewModel
                {
                    Id = r.Id,
                    Naziv = r.Naziv,
                    Tip = r.Tip,
                    Opis = r.Opis,
                    Cena = r.Cena
                }).ToList()
            };
        }

        public async Task<IEnumerable<Projekti>> GetByKorisnikIdAsync(Guid korisnikId)
        {
            return await _context.ClanoviProjekta
                .Where(cp => cp.KorisnikId == korisnikId)
                .Include(cp => cp.Projekat)
                    .ThenInclude(p => p.KreiraoKorisnik)
                .Select(cp => cp.Projekat)                
                .ToListAsync();
        }

        public async Task<Projekti> UpdateAsync(Projekti projekat)
        {
            var postojeciProjekat = await _context.Projektis.FirstOrDefaultAsync(p => p.Id == projekat.Id);

            if(postojeciProjekat == null)
            {
                throw new KeyNotFoundException("Projekat nije pronađen.");
            }

            postojeciProjekat.Naziv = projekat.Naziv;
            postojeciProjekat.Opis = projekat.Opis;
            postojeciProjekat.Status = projekat.Status;
            postojeciProjekat.Budzet = projekat.Budzet;
            postojeciProjekat.DatumPocetka = projekat.DatumPocetka;
            postojeciProjekat.Rok = projekat.Rok;
            postojeciProjekat.ZanrId = projekat.ZanrId;

            await _context.SaveChangesAsync();
            return postojeciProjekat;
        }

        public async Task<List<SelectListItem>> GetSelectOptionsAsync()
        {
            return await _context.Projektis
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Naziv
                })
                .ToListAsync();
        }
    }
}
