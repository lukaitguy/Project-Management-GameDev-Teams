using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Models.ViewModels;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Services
{
    public class AdminKorisniciService : IAdminKorisniciService
    {
        private readonly IAdminKorisniciRepository _repo;

        public AdminKorisniciService(IAdminKorisniciRepository repo)
        {
            _repo = repo;
        }

        public async Task<ServiceResult<IEnumerable<AdminKorisnikListDto>>> GetAllAsync(string? search, string? role)
        {
            var korisnici = await _repo.GetAllAsync(search, role);
            return ServiceResult<IEnumerable<AdminKorisnikListDto>>.Ok(korisnici.Select(MapToListDto));
        }

        public async Task<ServiceResult<AdminKorisnikListDto>> GetByIdAsync(string identityUserId)
        {
            var korisnik = await _repo.GetByIdentityIdAsync(identityUserId);
            if (korisnik == null)
                return ServiceResult<AdminKorisnikListDto>.Fail("Korisnik nije pronadjen.");

            return ServiceResult<AdminKorisnikListDto>.Ok(MapToListDto(korisnik));
        }

        public async Task<ServiceResult<IEnumerable<string>>> GetRolesAsync()
        {
            var roles = await _repo.GetAllRolesAsync();
            return ServiceResult<IEnumerable<string>>.Ok(roles);
        }

        public async Task<ServiceResult<bool>> CreateAsync(AdminCreateKorisnikDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return ServiceResult<bool>.Fail("Korisnicko ime je obavezno.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                return ServiceResult<bool>.Fail("Email je obavezan.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                return ServiceResult<bool>.Fail("Lozinka je obavezna.");

            try
            {
                await _repo.CreateAsync(new KreirajKorisnikaViewModel
                {
                    UserName = dto.UserName.Trim(),
                    Email = dto.Email.Trim(),
                    Password = dto.Password,
                    Roles = dto.Roles,
                    Ime = dto.Ime?.Trim(),
                    Prezime = dto.Prezime?.Trim(),
                    BrojTelefona = dto.BrojTelefona?.Trim(),
                    Biografija = dto.Biografija?.Trim(),
                    KreirajAppProfil = true
                });
            }
            catch (InvalidOperationException ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(string identityUserId, AdminUpdateKorisnikDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return ServiceResult<bool>.Fail("Korisnicko ime je obavezno.");
            if (string.IsNullOrWhiteSpace(dto.Email))
                return ServiceResult<bool>.Fail("Email je obavezan.");

            var existing = await _repo.GetByIdentityIdAsync(identityUserId);
            if (existing == null)
                return ServiceResult<bool>.Fail("Korisnik nije pronadjen.");

            try
            {
                await _repo.UpdateAsync(new KorisniciViewModel
                {
                    IdentityUserId = identityUserId,
                    Id = existing.Id,
                    UserName = dto.UserName.Trim(),
                    Email = dto.Email.Trim(),
                    Roles = dto.Roles,
                    Ime = dto.Ime?.Trim(),
                    Prezime = dto.Prezime?.Trim(),
                    BrojTelefona = dto.BrojTelefona?.Trim(),
                    Biografija = dto.Biografija?.Trim()
                });
            }
            catch (KeyNotFoundException ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> ChangePasswordAsync(string identityUserId, AdminPromeniLozinkuDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NovaLozinka))
                return ServiceResult<bool>.Fail("Nova lozinka je obavezna.");

            try
            {
                await _repo.ChangePasswordAsync(new PromenaLozinkeViewModel
                {
                    IdentityUserId = identityUserId,
                    NewPassword = dto.NovaLozinka,
                    ConfirmPassword = dto.NovaLozinka
                });
            }
            catch (KeyNotFoundException ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string identityUserId)
        {
            try
            {
                await _repo.DeleteAsync(identityUserId);
            }
            catch (InvalidOperationException ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private static AdminKorisnikListDto MapToListDto(KorisniciViewModel k) => new()
        {
            IdentityUserId = k.IdentityUserId,
            UserName = k.UserName,
            Email = k.Email,
            Roles = k.Roles,
            Ime = k.Ime,
            Prezime = k.Prezime,
            BrojTelefona = k.BrojTelefona,
            Biografija = k.Biografija,
            ProjektiCount = k.ProjektiCount,
            ZadaciCount = k.ZadaciCount
        };
    }
}
