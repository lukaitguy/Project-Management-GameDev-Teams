using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PMDbContext _pmDbContext;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<IdentityUser> userManager, PMDbContext pmDbContext, ITokenService tokenService)
        {
            _userManager = userManager;
            _pmDbContext = pmDbContext;
            _tokenService = tokenService;
        }

        public async Task<ServiceResult<object>> PrijavaAsync(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.EmailIliKorisnickoIme) ||
                string.IsNullOrWhiteSpace(request.Lozinka))
            {
                return ServiceResult<object>.Fail(
                    "Email/korisničko ime i lozinka su obavezni.");
            }

            var korisnik =
                await _userManager.FindByEmailAsync(
                    request.EmailIliKorisnickoIme)
                ?? await _userManager.FindByNameAsync(
                    request.EmailIliKorisnickoIme);

            if (korisnik == null ||
                !await _userManager.CheckPasswordAsync(
                    korisnik,
                    request.Lozinka))
            {
                return ServiceResult<object>.Fail(
                    "Pogrešni podaci za prijavu.");
            }

            var roles = await _userManager.GetRolesAsync(korisnik);

            var korisnikProfil = await _pmDbContext.Korisnicis
                .FirstOrDefaultAsync(k => k.IdentityUserId == korisnik.Id);

            var token = _tokenService.GenerateJwtToken(korisnik, roles, korisnikProfil?.Id);

            return ServiceResult<object>.Ok(new
            {
                token,
                korisnik = new
                {
                    korisnickoIme = korisnik.UserName,
                    email = korisnik.Email,
                    isAdmin = roles.Contains("Administrator"),
                    isPM = roles.Contains("ProjektniMenadzer")
                }
            });
        }

        public async Task<ServiceResult<object>> RegistracijaAsync(RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Ime) ||
                string.IsNullOrWhiteSpace(request.Prezime) ||
                string.IsNullOrWhiteSpace(request.KorisnickoIme) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Lozinka))
            {
                return ServiceResult<object>.Fail(
                    "Sva obavezna polja moraju biti popunjena.");
            }

            var postojeciEmail =
                await _userManager.FindByEmailAsync(request.Email);

            if (postojeciEmail != null)
            {
                return ServiceResult<object>.Fail(
                    "Email adresa je već zauzeta.");
            }

            var postojeciKorisnik =
                await _userManager.FindByNameAsync(request.KorisnickoIme);

            if (postojeciKorisnik != null)
            {
                return ServiceResult<object>.Fail(
                    "Korisničko ime je već zauzeto.");
            }

            var identityUser = new IdentityUser
            {
                UserName = request.KorisnickoIme,
                Email = request.Email
            };

            var rezultat = await _userManager.CreateAsync(
                identityUser,
                request.Lozinka);

            if (!rezultat.Succeeded)
            {
                return ServiceResult<object>.Fail(
                    string.Join(" | ",
                        rezultat.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(identityUser, "Korisnik");

            try
            {
                var korisnik = new Korisnici
                {
                    Id = Guid.NewGuid(),
                    IdentityUserId = identityUser.Id,
                    Ime = request.Ime,
                    Prezime = request.Prezime,
                    Email = request.Email,
                    BrojTelefona = request.BrojTelefona,
                    DatumKreiranja = DateTime.UtcNow
                };

                _pmDbContext.Korisnicis.Add(korisnik);

                await _pmDbContext.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(identityUser);

                return ServiceResult<object>.Fail(
                    "Korisnik neuspešno kreiran u aplikacionoj bazi.");
            }

            return ServiceResult<object>.Ok("Registracija je uspešno završena.");
        }

    }
}
