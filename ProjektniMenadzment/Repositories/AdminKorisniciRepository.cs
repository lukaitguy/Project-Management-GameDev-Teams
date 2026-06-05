using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Repositories
{
    public class AdminKorisniciRepository : IAdminKorisniciRepository
    {
        private readonly PMAuthDbContext _auth;
        private readonly PMDbContext _app;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminKorisniciRepository(
            PMAuthDbContext auth,
            PMDbContext app,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _auth = auth;
            _app = app;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ChangePasswordAsync(PromenaLozinkeViewModel lozinka)
        {
            var u = await _userManager.FindByIdAsync(lozinka.IdentityUserId) ?? throw new KeyNotFoundException("Identity korisnik nije pronađen.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(u);
            var res = await _userManager.ResetPasswordAsync(u, token, lozinka.NewPassword);
            if (!res.Succeeded)
                throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
        }

        public async Task DeleteAsync(string identityUserId)
        {
            var k = await _app.Korisnicis.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId);
            if (k != null)
            {
                var clanovi = _app.ClanoviProjekta.Where(c => c.KorisnikId == k.Id);
                _app.ClanoviProjekta.RemoveRange(clanovi);

                var komentari = _app.KomentariZadataks.Where(c => c.KorisnikId == k.Id);
                _app.KomentariZadataks.RemoveRange(komentari);

                var zadaci = await _app.Zadacis.Where(z => z.DodeljenKorisnikuId == k.Id).ToListAsync();
                foreach (var z in zadaci) z.DodeljenKorisnikuId = null;

                await _app.SaveChangesAsync();

                _app.Korisnicis.Remove(k);
                await _app.SaveChangesAsync();
            }

            var u = await _userManager.FindByIdAsync(identityUserId);
            if (u != null)
            {
                var res = await _userManager.DeleteAsync(u);
                if (!res.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
            }
        }
        public async Task<List<KorisniciViewModel>> GetAllAsync(string? search = null, string? role = null, bool onlyLinked = false)
        {
            var identityUsers = await _userManager.Users.AsNoTracking().ToListAsync();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.Trim().ToLower();
                identityUsers = identityUsers
                    .Where(u => u.UserName!.ToLower().Contains(s) || u.Email!.ToLower().Contains(s))
                    .ToList();
            }

            var appKorisnici = await _app.Korisnicis.AsNoTracking().ToListAsync();
            var appByIdentity = appKorisnici.ToDictionary(k => k.IdentityUserId, k => k);

            var projektiByKreirao = await _app.Projektis
                .AsNoTracking()
                .GroupBy(p => p.KreiraoKorisnikId)
                .Select(g => new { KorisnikId = g.Key, Cnt = g.Count() })
                .ToListAsync();
            var zadaciByDodeljen = await _app.Zadacis
                .AsNoTracking()
                .Where(z => z.DodeljenKorisnikuId != null)
                .GroupBy(z => z.DodeljenKorisnikuId!.Value)
                .Select(g => new { KorisnikId = g.Key, Cnt = g.Count() })
                .ToListAsync();
            var projDict = projektiByKreirao.ToDictionary(x => x.KorisnikId, x => x.Cnt);
            var zadDict = zadaciByDodeljen.ToDictionary(x => x.KorisnikId, x => x.Cnt);

            var list = new List<KorisniciViewModel>(identityUsers.Count);
            foreach (var u in identityUsers)
            {
                var roles = (await _userManager.GetRolesAsync(u)).ToArray();

                if (!string.IsNullOrWhiteSpace(role) && !roles.Contains(role))
                    continue;

                appByIdentity.TryGetValue(u.Id, out var k);

                var vm = new KorisniciViewModel
                {
                    IdentityUserId = u.Id,
                    UserName = u.UserName ?? "",
                    Email = u.Email ?? "",
                    Roles = roles,

                    Id = k?.Id,
                    Ime = k?.Ime,
                    Prezime = k?.Prezime,
                    BrojTelefona = k?.BrojTelefona,
                    Biografija = k?.Biografija,

                    ProjektiCount = (k != null && projDict.TryGetValue(k.Id, out var pc)) ? pc : 0,
                    ZadaciCount = (k != null && zadDict.TryGetValue(k.Id, out var zc)) ? zc : 0
                };

                list.Add(vm);
            }

            if (onlyLinked)
                list = list.Where(x => x.isLinked).ToList();


            return list.OrderBy(x => x.UserName).ToList();
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
        }

        public async Task<KorisniciViewModel?> GetByIdentityIdAsync(string identityUserId)
        {
            var u = await _userManager.FindByIdAsync(identityUserId);
            if (u == null) return null;

            var roles = await _userManager.GetRolesAsync(u);
            var k = await _app.Korisnicis.AsNoTracking().FirstOrDefaultAsync(x => x.IdentityUserId == u.Id);

            // brojanja
            var projektiCnt = k != null ? await _app.Projektis.CountAsync(p => p.KreiraoKorisnikId == k.Id) : 0;
            var zadaciCnt = k != null ? await _app.Zadacis.CountAsync(z => z.DodeljenKorisnikuId == k.Id) : 0;

            return new KorisniciViewModel
            {
                IdentityUserId = u.Id,
                UserName = u.UserName ?? "",
                Email = u.Email ?? "",
                Roles = roles.ToArray(),

                Id = k?.Id,
                Ime = k?.Ime,
                Prezime = k?.Prezime,
                BrojTelefona = k?.BrojTelefona,
                Biografija = k?.Biografija,

                ProjektiCount = projektiCnt,
                ZadaciCount = zadaciCnt
            };
        }

        public async Task UpdateAsync(KorisniciViewModel korisnik)
        {
            var u = await _userManager.FindByIdAsync(korisnik.IdentityUserId)
                    ?? throw new KeyNotFoundException("Identity korisnik nije pronađen.");

            u.UserName = korisnik.UserName;
            u.Email = korisnik.Email;
            var idRes = await _userManager.UpdateAsync(u);
            if (!idRes.Succeeded)
                throw new InvalidOperationException(string.Join("; ", idRes.Errors.Select(e => e.Description)));

            var currentRoles = await _userManager.GetRolesAsync(u);
            var toRemove = currentRoles.Except(korisnik.Roles).ToArray();
            var toAdd = korisnik.Roles.Except(currentRoles).ToArray();
            if (toRemove.Length > 0) await _userManager.RemoveFromRolesAsync(u, toRemove);
            if (toAdd.Length > 0) await _userManager.AddToRolesAsync(u, toAdd);

            var hasAppId = korisnik.Id.HasValue && korisnik.Id.Value != Guid.Empty;

            if (hasAppId)
            {
                var k = await _app.Korisnicis.FindAsync(korisnik.Id!.Value);

                if (k == null)
                    k = await _app.Korisnicis.FirstOrDefaultAsync(x => x.IdentityUserId == korisnik.IdentityUserId);

                if (k == null)
                    throw new KeyNotFoundException("App korisnik nije pronadjen.");

                k.Ime = korisnik.Ime ?? k.Ime;
                k.Prezime = korisnik.Prezime ?? k.Prezime;
                k.Email = korisnik.Email;
                k.BrojTelefona = korisnik.BrojTelefona;
                k.Biografija = korisnik.Biografija;

                await _app.SaveChangesAsync();
            }
            else
            {
                var k = await _app.Korisnicis.FirstOrDefaultAsync(x => x.IdentityUserId == korisnik.IdentityUserId);
                if (k == null)
                {
                    _app.Korisnicis.Add(new Korisnici
                    {
                        Id = Guid.NewGuid(),
                        Ime = korisnik.Ime ?? "",
                        Prezime = korisnik.Prezime ?? "",
                        Email = korisnik.Email,
                        BrojTelefona = korisnik.BrojTelefona,
                        Biografija = korisnik.Biografija,
                        DatumKreiranja = DateTime.UtcNow,
                        IdentityUserId = korisnik.IdentityUserId
                    });
                }
                else
                {
                    k.Ime = korisnik.Ime ?? k.Ime;
                    k.Prezime = korisnik.Prezime ?? k.Prezime;
                    k.Email = korisnik.Email;
                    k.BrojTelefona = korisnik.BrojTelefona;
                    k.Biografija = korisnik.Biografija;
                }
                await _app.SaveChangesAsync();
            }
        }

        public async Task CreateAsync(KreirajKorisnikaViewModel korisnik)
        {
            var identityUser = new IdentityUser
            {
                UserName = korisnik.UserName,
                Email = korisnik.Email
            };

            var createRes = await _userManager.CreateAsync(identityUser, korisnik.Password);
            if (!createRes.Succeeded)
                throw new InvalidOperationException(string.Join("; ", createRes.Errors.Select(e => e.Description)));

            var roles = (korisnik.Roles?.Length > 0 ? korisnik.Roles : new[] { "Korisnik" });
            var knownRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
            var toAssign = roles.Intersect(knownRoles, StringComparer.OrdinalIgnoreCase).ToArray();
            if (toAssign.Any())
            {
                var addRolesRes = await _userManager.AddToRolesAsync(identityUser, toAssign);
                if (!addRolesRes.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", addRolesRes.Errors.Select(e => e.Description)));
            }

            if (korisnik.KreirajAppProfil)
            {
                var appUser = new Korisnici
                {
                    Id = Guid.NewGuid(),
                    IdentityUserId = identityUser.Id,
                    Ime = korisnik.Ime ?? "",
                    Prezime = korisnik.Prezime ?? "",
                    BrojTelefona = korisnik.BrojTelefona,
                    Biografija = korisnik.Biografija,
                    Email = korisnik.Email,        
                    DatumKreiranja = DateTime.UtcNow
                };
                _app.Korisnicis.Add(appUser);
                await _app.SaveChangesAsync();
            }
        }
    }
}
