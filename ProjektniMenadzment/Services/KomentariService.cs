using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Services
{
    public class KomentariService : IKomentariService
    {
        private readonly IZadaciKomentarRepository _komentarRepository;
        private readonly IKorisniciRepository _korisniciRepository;

        public KomentariService(
            IZadaciKomentarRepository komentarRepository,
            IKorisniciRepository korisniciRepository)
        {
            _komentarRepository = komentarRepository;
            _korisniciRepository = korisniciRepository;
        }

        public async Task<ServiceResult<IEnumerable<KomentarDto>>> GetByZadatakIdAsync(Guid zadatakId)
        {
            var komentari = await _komentarRepository.GetAllByIdAsync(zadatakId);
            return ServiceResult<IEnumerable<KomentarDto>>.Ok(komentari.Select(MapToDto));
        }

        public async Task<ServiceResult<KomentarDto>> AddAsync(
            Guid zadatakId, CreateKomentarDto dto, string identityUserId)
        {
            if (string.IsNullOrWhiteSpace(dto.Sadrzaj))
                return ServiceResult<KomentarDto>.Fail("Sadržaj komentara je obavezan.");

            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUserId);
            if (korisnik == null)
                return ServiceResult<KomentarDto>.Fail("Korisnik nije pronadjen.");

            var komentar = new KomentariZadatak
            {
                Id = Guid.NewGuid(),
                Sadrzaj = dto.Sadrzaj.Trim(),
                ZadatakId = zadatakId,
                KorisnikId = korisnik.Id,
                DatumKreiranja = DateTime.UtcNow
            };

            try
            {
                await _komentarRepository.AddAsync(komentar);
            }
            catch
            {
                return ServiceResult<KomentarDto>.Fail("Greška pri čuvanju komentara.");
            }

            // Manually set navigation for return DTO since repo returns without Include
            komentar.Korisnik = korisnik;
            return ServiceResult<KomentarDto>.Ok(MapToDto(komentar));
        }

        private static KomentarDto MapToDto(KomentariZadatak k) => new()
        {
            Id = k.Id,
            Sadrzaj = k.Sadrzaj,
            ZadatakId = k.ZadatakId,
            KorisnikId = k.KorisnikId,
            KorisnikIme = $"{k.Korisnik.Ime} {k.Korisnik.Prezime}",
            DatumKreiranja = k.DatumKreiranja
        };
    }
}
