using Microsoft.AspNetCore.Identity;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;
using System.Security.Claims;

namespace ProjektniMenadzment.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IKorisniciRepository _korisniciRepository;
        private readonly ITokenService _tokenService;

        public AccountService(
            UserManager<IdentityUser> userManager,
            IKorisniciRepository korisniciRepository,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _korisniciRepository = korisniciRepository;
            _tokenService = tokenService;
        }

        public async Task<ServiceResult<ProfilDto>> GetProfilAsync(ClaimsPrincipal user)
        {
            var identityUser = await _userManager.GetUserAsync(user);
            if (identityUser == null)
                return ServiceResult<ProfilDto>.Fail("Korisnik nije pronađen.");

            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUser.Id);
            if (korisnik == null)
                return ServiceResult<ProfilDto>.Fail("Profil nije pronađen.");

            return ServiceResult<ProfilDto>.Ok(new ProfilDto
            {
                KorisnickoIme = identityUser.UserName!,
                Email = identityUser.Email!,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                BrojTelefona = korisnik.BrojTelefona,
                Biografija = korisnik.Biografija,
                DatumKreiranja = korisnik.DatumKreiranja
            });
        }

        public async Task<ServiceResult<object>> UpdateProfilAsync(ClaimsPrincipal user, UpdateProfilDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Ime) || string.IsNullOrWhiteSpace(dto.Prezime))
                return ServiceResult<object>.Fail("Ime i prezime su obavezni.");

            var identityUser = await _userManager.GetUserAsync(user);
            if (identityUser == null)
                return ServiceResult<object>.Fail("Korisnik nije pronađen.");

            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUser.Id);
            if (korisnik == null)
                return ServiceResult<object>.Fail("Profil nije pronađen.");

            korisnik.Ime = dto.Ime;
            korisnik.Prezime = dto.Prezime;
            korisnik.BrojTelefona = dto.BrojTelefona;
            korisnik.Biografija = dto.Biografija;

            await _korisniciRepository.UpdateAsync(korisnik);

            return ServiceResult<object>.Ok("Profil je uspešno ažuriran.");
        }

        public async Task<ServiceResult<object>> PromeniKorisnickoImeAsync(ClaimsPrincipal user, PromeniKorisnickoImeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NovoKorisnickoIme))
                return ServiceResult<object>.Fail("Korisničko ime ne sme biti prazno.");

            var identityUser = await _userManager.GetUserAsync(user);
            if (identityUser == null)
                return ServiceResult<object>.Fail("Korisnik nije pronađen.");

            var existing = await _userManager.FindByNameAsync(dto.NovoKorisnickoIme);
            if (existing != null && existing.Id != identityUser.Id)
                return ServiceResult<object>.Fail("Korisničko ime je već zauzeto.");

            var rezultat = await _userManager.SetUserNameAsync(identityUser, dto.NovoKorisnickoIme);
            if (!rezultat.Succeeded)
                return ServiceResult<object>.Fail(
                    string.Join(" | ", rezultat.Errors.Select(e => e.Description)));

            var roles = await _userManager.GetRolesAsync(identityUser);
            var korisnikProfil = await _korisniciRepository.GetByIdentityUserIdAsync(identityUser.Id);
            var token = _tokenService.GenerateJwtToken(identityUser, roles, korisnikProfil?.Id);

            return ServiceResult<object>.Ok(new
            {
                token,
                korisnickoIme = identityUser.UserName,
                isAdmin = roles.Contains("Administrator"),
                isPM = roles.Contains("ProjektniMenadzer")
            });
        }

        public async Task<ServiceResult<object>> PromeniLozinkuAsync(ClaimsPrincipal user, PromeniLozinkuDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TrenutnaLozinka) || string.IsNullOrWhiteSpace(dto.NovaLozinka))
                return ServiceResult<object>.Fail("Lozinke ne smeju biti prazne.");

            var identityUser = await _userManager.GetUserAsync(user);
            if (identityUser == null)
                return ServiceResult<object>.Fail("Korisnik nije pronađen.");

            var rezultat = await _userManager.ChangePasswordAsync(
                identityUser, dto.TrenutnaLozinka, dto.NovaLozinka);

            if (!rezultat.Succeeded)
                return ServiceResult<object>.Fail(
                    string.Join(" | ", rezultat.Errors.Select(e => e.Description)));

            return ServiceResult<object>.Ok("Lozinka je uspešno promenjena.");
        }
    }
}
