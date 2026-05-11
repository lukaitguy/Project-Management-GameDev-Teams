using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjektniMenadzment.Data;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PMDbContext _pmDbContext;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUser> userManager,
            PMDbContext pmDbContext,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _pmDbContext = pmDbContext;
            _configuration = configuration;
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

            if (korisnik == null || !await _userManager.CheckPasswordAsync(korisnik, request.Lozinka))
                return Unauthorized(new { message = "Pogrešni podaci za prijavu." });

            var roles = await _userManager.GetRolesAsync(korisnik);
            var token = GenerateJwtToken(korisnik, roles);

            return Ok(new
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

        [HttpGet("trenutni-korisnik")]
        [Authorize]
        public async Task<IActionResult> TrenutniKorisnik()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null)
                return Unauthorized(new { message = "Korisnik nije prijavljen." });

            var roles = await _userManager.GetRolesAsync(korisnik);

            return Ok(new
            {
                korisnickoIme = korisnik.UserName,
                email = korisnik.Email,
                isAdmin = roles.Contains("Administrator"),
                isPM = roles.Contains("ProjektniMenadzer")
            });
        }

        private string GenerateJwtToken(IdentityUser korisnik, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, korisnik.Id),
                new(ClaimTypes.Name, korisnik.UserName!),
                new(ClaimTypes.Email, korisnik.Email!)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpiresInMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}