using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Services
{
    public class ResursiService : IResursiService
    {
        private readonly IResursiRepository _resursiRepository;

        public ResursiService(IResursiRepository resursiRepository)
        {
            _resursiRepository = resursiRepository;
        }

        public async Task<ServiceResult<Guid>> CreateAsync(Guid projekatId, CreateResursDto dto)
        {
            var validateResurs = ValidateResurs(dto.Naziv, dto.Tip);
            if(validateResurs != null)
            {
                return ServiceResult<Guid>.Fail(validateResurs);
            }

            var resurs = new Resursi
            {
                Id = Guid.NewGuid(),
                Naziv = dto.Naziv.Trim(),
                Tip = dto.Tip.Trim(),
                Opis = CleanText(dto.Opis),
                Cena = dto.Cena,
                ProjekatId = projekatId,
                DodeljenKorisniku = dto.DodeljenKorisniku,
                DatumKreiranja = DateTime.UtcNow
            };

            try
            {
                await _resursiRepository.AddAsync(resurs);
            }
            catch (DbUpdateException)
            {
                return ServiceResult<Guid>.Fail("Greska pri kreiranju resursa.");
            }

            return ServiceResult<Guid>.Ok(resurs.Id);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var obrisan = await _resursiRepository.DeleteAsync(id);
                if (!obrisan)
                {
                    return ServiceResult<bool>.Fail("Resurs nije pronadjen.");
                }
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Resurs nije obrisan. Greska.");
            }

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<ResursListDto>> GetByIdAsync(Guid id)
        {
            var resurs = await _resursiRepository.GetByIdAsync(id);
            if (resurs == null)
                return ServiceResult<ResursListDto>.Fail("Resurs nije pronadjen.");
            return ServiceResult<ResursListDto>.Ok(MapToDto(resurs));
        }

        public async Task<ServiceResult<IEnumerable<ResursListDto>>> GetByProjekatIdAsync(Guid projekatId)
        {
            var resursi = await _resursiRepository.GetByProjekatIdAsync(projekatId);
            return ServiceResult<IEnumerable<ResursListDto>>.Ok(resursi.Select(MapToDto));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(Guid id, UpdateResursDto dto)
        {
            var validationMessage = ValidateResurs(dto.Naziv, dto.Tip);
            if (validationMessage != null)
                return ServiceResult<bool>.Fail(validationMessage);

            var resurs = await _resursiRepository.GetByIdAsync(id);
            if (resurs == null)
                return ServiceResult<bool>.Fail("Resurs nije pronadjen.");

            resurs.Naziv = dto.Naziv.Trim();
            resurs.Tip = dto.Tip.Trim();
            resurs.Opis = CleanText(dto.Opis);
            resurs.Cena = dto.Cena;
            resurs.DodeljenKorisniku = dto.DodeljenKorisniku;

            try
            {
                await _resursiRepository.UpdateAsync(resurs);
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Greška pri izmeni resursa.");
            }

            return ServiceResult<bool>.Ok(true);
        }

        private static ResursListDto MapToDto(Resursi r) => new()
        {
            Id = r.Id,
            Naziv = r.Naziv,
            Tip = r.Tip,
            Opis = r.Opis,
            Cena = r.Cena,
            DodeljenKorisniku = r.DodeljenKorisniku,
            DodeljenKorisnikuIme = r.DodeljenKorisnikuNavigation != null ? $"{r.DodeljenKorisnikuNavigation.Ime} {r.DodeljenKorisnikuNavigation.Prezime}" : null,
            DatumKreiranja = r.DatumKreiranja,
        };

        private static string? ValidateResurs(string naziv, string tip)
        {
            if (string.IsNullOrWhiteSpace(naziv))
                return "Naziv resursa je obavezan.";
            if (string.IsNullOrWhiteSpace(tip))
                return "Tip resursa je obavezan.";
            return null;
        }

        private static string? CleanText(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
