using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly PMDbContext _pmDbContext;

        public AuthController(UserManager<IdentityUser> userManager, 
                              SignInManager<IdentityUser> signInManager,
                              PMDbContext pmDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _pmDbContext = pmDbContext;
        }

        [HttpPost("registracija")]
        public async Task<IActionResult> Registracija([FromBody] RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Ime) ||
                string.IsNullOrWhiteSpace(request.Prezime) ||
                string.IsNullOrWhiteSpace(request.KorisnickoIme) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Lozinka))
            {
                return BadRequest(new { message = "Sva obavezna polja moraju biti popunjena." });
            }

            var postojeciEmail = await _userManager.FindByEmailAsync(request.Email);
            if (postojeciEmail != null)
                return BadRequest(new { message = "Email adresa je već zauzeta." });

            var postojeciKorisnik = await _userManager.FindByNameAsync(request.KorisnickoIme);
            if (postojeciKorisnik != null)
                return BadRequest(new { message = "Korisničko ime je već zauzeto." });

            var identityUser = new IdentityUser
            {
                UserName = request.KorisnickoIme,
                Email = request.Email
            };

            var rezultat = await _userManager.CreateAsync(identityUser, request.Lozinka);

            if (!rezultat.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Registracija nije uspela.",
                    errors = rezultat.Errors.Select(e => e.Description)
                });
            }

            //Provera upisa u aplikacionu bazu
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
                return StatusCode(500, new { message = "Korisnik neuspesno kreiran u aplikacionoj bazi." });
            }


            return Ok(new { message = "Registracija je uspešno završena." });
        }

        [HttpPost("prijava")]
        public async Task<IActionResult> Prijava([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.EmailIliKorisnickoIme) ||
                string.IsNullOrWhiteSpace(request.Lozinka))
            {
                return BadRequest(new { message = "Email/korisničko ime i lozinka su obavezni." });
            }

            var korisnik = await _userManager.FindByEmailAsync(request.EmailIliKorisnickoIme)
                          ?? await _userManager.FindByNameAsync(request.EmailIliKorisnickoIme);

            if (korisnik == null)
            {
                return Unauthorized(new { message = "Pogrešni podaci za prijavu." });
            }

            var rezultat = await _signInManager.PasswordSignInAsync(
                korisnik.UserName!,
                request.Lozinka,
                request.ZapamtiMe,
                lockoutOnFailure: false);

            if (!rezultat.Succeeded)
            {
                return Unauthorized(new { message = "Pogrešni podaci za prijavu." });
            }

            return Ok(new
            {
                message = "Uspešna prijava.",
                korisnik = new
                {
                    korisnickoIme = korisnik.UserName,
                    email = korisnik.Email
                }
            });
        }
    }
}
