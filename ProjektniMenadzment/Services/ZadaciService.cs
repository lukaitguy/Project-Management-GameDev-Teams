using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Services
{
    public class ZadaciService : IZadaciService
    {
        private readonly IZadaciRepository _zadaciRepository;
        private readonly IKorisniciRepository _korisniciRepository;
        private readonly IProjektiRepository _projektiRepository;
        private readonly IClanoviProjektaRepository _clanoviProjektaRepository;

        public ZadaciService(
            IZadaciRepository zadaciRepository,
            IKorisniciRepository korisniciRepository,
            IProjektiRepository projektiRepository,
            IClanoviProjektaRepository clanoviProjektaRepository)
        {
            _zadaciRepository = zadaciRepository;
            _korisniciRepository = korisniciRepository;
            _projektiRepository = projektiRepository;
            _clanoviProjektaRepository = clanoviProjektaRepository;
        }

        public async Task<ServiceResult<IEnumerable<ZadatakListDto>>> GetByProjekatIdAsync(Guid projekatId)
        {
            var zadaci = await _zadaciRepository.GetByProjekatIdAsync(projekatId);
            return ServiceResult<IEnumerable<ZadatakListDto>>.Ok(zadaci.Select(MapToListDto));
        }

        public async Task<ServiceResult<IEnumerable<MojZadatakDto>>> GetMojiZadaciAsync(string identityUserId)
        {
            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUserId);
            if (korisnik == null)
                return ServiceResult<IEnumerable<MojZadatakDto>>.Fail("Korisnik nije pronadjen.");

            var zadaci = await _zadaciRepository.GetByKorisnikIdAsync(korisnik.Id);
            return ServiceResult<IEnumerable<MojZadatakDto>>.Ok(zadaci.Select(MapToMojZadatakDto));
        }

        public async Task<ServiceResult<ZadatakDetailsDto>> GetByIdAsync(Guid id)
        {
            var zadatak = await _zadaciRepository.GetByIdAsync(id);
            if (zadatak == null)
                return ServiceResult<ZadatakDetailsDto>.Fail("Zadatak nije pronadjen.");

            return ServiceResult<ZadatakDetailsDto>.Ok(MapToDetailsDto(zadatak));
        }

        public async Task<ServiceResult<Guid>> CreateAsync(Guid projekatId, CreateZadatakDto dto, string identityUserId)
        {
            var validationMessage = ValidateZadatak(dto.Naslov, dto.Status, dto.Prioritet, dto.Opis, dto.TipZadatka);
            if (validationMessage != null)
                return ServiceResult<Guid>.Fail(validationMessage);

            var projekat = await _projektiRepository.GetByIdAsync(projekatId);
            if (projekat == null)
                return ServiceResult<Guid>.Fail("Projekat nije pronadjen.");

            validationMessage = ValidateRokUnutarProjekta(dto.Rok, projekat.DatumPocetka, projekat.Rok);
            if (validationMessage != null)
                return ServiceResult<Guid>.Fail(validationMessage);

            if (dto.DodeljenKorisnikuId.HasValue)
            {
                var clanoviProjekta = await _clanoviProjektaRepository.GetKorisniciByProjekatIdAsync(projekatId);
                validationMessage = ValidateDodeljenKorisnik(dto.DodeljenKorisnikuId, clanoviProjekta);
                if (validationMessage != null)
                    return ServiceResult<Guid>.Fail(validationMessage);
            }

            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUserId);
            if (korisnik == null)
                return ServiceResult<Guid>.Fail("Aplikacioni profil korisnika nije pronadjen.");

            var zadatak = new Zadaci
            {
                Id = Guid.NewGuid(),
                ProjekatId = projekatId,
                Naslov = dto.Naslov.Trim(),
                Opis = CleanText(dto.Opis),
                Status = dto.Status.Trim(),
                Prioritet = dto.Prioritet.Trim(),
                TipZadatka = CleanText(dto.TipZadatka),
                Rok = dto.Rok,
                DodeljenKorisnikuId = dto.DodeljenKorisnikuId,
                KreiraoKorisnikId = korisnik.Id,
                DatumKreiranja = DateTime.UtcNow
            };

            try
            {
                await _zadaciRepository.AddAsync(zadatak);
            }
            catch (DbUpdateException)
            {
                return ServiceResult<Guid>.Fail("Doslo je do greske pri cuvanju zadatka.");
            }

            return ServiceResult<Guid>.Ok(zadatak.Id);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(Guid id, UpdateZadatakDto dto)
        {
            var validationMessage = ValidateZadatak(dto.Naslov, dto.Status, dto.Prioritet, dto.Opis, dto.TipZadatka);
            if (validationMessage != null)
                return ServiceResult<bool>.Fail(validationMessage);

            var zadatak = await _zadaciRepository.GetByIdAsync(id);
            if (zadatak == null)
                return ServiceResult<bool>.Fail("Zadatak nije pronadjen.");

            var projekat = await _projektiRepository.GetByIdAsync(zadatak.ProjekatId);
            if (projekat == null)
                return ServiceResult<bool>.Fail("Projekat nije pronadjen.");

            validationMessage = ValidateRokUnutarProjekta(dto.Rok, projekat.DatumPocetka, projekat.Rok);
            if (validationMessage != null)
                return ServiceResult<bool>.Fail(validationMessage);

            if (dto.DodeljenKorisnikuId.HasValue)
            {
                var clanoviProjekta = await _clanoviProjektaRepository.GetKorisniciByProjekatIdAsync(zadatak.ProjekatId);
                validationMessage = ValidateDodeljenKorisnik(dto.DodeljenKorisnikuId, clanoviProjekta);
                if (validationMessage != null)
                    return ServiceResult<bool>.Fail(validationMessage);
            }

            zadatak.Naslov = dto.Naslov.Trim();
            zadatak.Opis = CleanText(dto.Opis);
            zadatak.Status = dto.Status.Trim();
            zadatak.Prioritet = dto.Prioritet.Trim();
            zadatak.TipZadatka = CleanText(dto.TipZadatka);
            zadatak.Rok = dto.Rok;
            zadatak.DodeljenKorisnikuId = dto.DodeljenKorisnikuId;

            try
            {
                await _zadaciRepository.UpdateAsync(zadatak);
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Doslo je do greske pri izmeni zadatka.");
            }

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> UpdateStatusAsync(
            Guid id, string noviStatus, string identityUserId, bool isAdmin)
        {
            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUserId);
            if (korisnik == null)
                return ServiceResult<bool>.Fail("Korisnik nije pronadjen.");

            var uspesno = await _zadaciRepository.UpdateStatusAsync(
                id, korisnik.Id, noviStatus, isAdmin);

            return uspesno
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.Fail("Status nije moguce promeniti. Proverite dozvole ili vrednost statusa.");
        }

        public async Task<ServiceResult<bool>> DodeliZadatakAsync(Guid id, Guid korisnikId)
        {
            var uspesno = await _zadaciRepository.DodeliZadatakAsync(id, korisnikId);
            return uspesno
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.Fail("Zadatak nije pronadjen.");
        }

        public async Task<ServiceResult<bool>> PreuzmiZadatakAsync(Guid id, string identityUserId)
        {
            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUserId);
            if (korisnik == null)
                return ServiceResult<bool>.Fail("Korisnik nije pronadjen.");

            var uspesno = await _zadaciRepository.PreuzmiAsync(id, korisnik.Id);
            return uspesno
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.Fail("Zadatak nije dostupan za preuzimanje.");
        }

        public async Task<ServiceResult<bool>> OdustaniOdZadatkaAsync(Guid id, string identityUserId)
        {
            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUserId);
            if (korisnik == null)
                return ServiceResult<bool>.Fail("Korisnik nije pronadjen.");

            var uspesno = await _zadaciRepository.OdustaniAsync(id, korisnik.Id);
            return uspesno
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.Fail("Niste dodeljeni ovom zadatku.");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var uspesno = await _zadaciRepository.DeleteAsync(id);
                if (!uspesno)
                    return ServiceResult<bool>.Fail("Zadatak nije pronadjen.");
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Zadatak ne moze da se obrise dok postoje povezani podaci.");
            }

            return ServiceResult<bool>.Ok(true);
        }

        // ── Mappers ──────────────────────────────────────────────

        private static MojZadatakDto MapToMojZadatakDto(Zadaci z) => new()
        {
            Id = z.Id,
            ProjekatId = z.ProjekatId,
            ProjekatNaziv = z.Projekat.Naziv,
            Naslov = z.Naslov,
            Opis = z.Opis,
            Status = z.Status,
            Prioritet = z.Prioritet,
            TipZadatka = z.TipZadatka,
            Rok = z.Rok,
            DatumKreiranja = z.DatumKreiranja,
            DodeljenKorisnikuId = z.DodeljenKorisnikuId,
            DodeljenKorisnikuIme = z.DodeljenKorisniku != null
                ? $"{z.DodeljenKorisniku.Ime} {z.DodeljenKorisniku.Prezime}"
                : null
        };

        private static ZadatakListDto MapToListDto(Zadaci z) => new()
        {
            Id = z.Id,
            ProjekatId = z.ProjekatId,
            Naslov = z.Naslov,
            Opis = z.Opis,
            Status = z.Status,
            Prioritet = z.Prioritet,
            TipZadatka = z.TipZadatka,
            Rok = z.Rok,
            DatumKreiranja = z.DatumKreiranja,
            DodeljenKorisnikuId = z.DodeljenKorisnikuId,
            DodeljenKorisnikuIme = z.DodeljenKorisniku != null
                ? $"{z.DodeljenKorisniku.Ime} {z.DodeljenKorisniku.Prezime}"
                : null
        };

        private static ZadatakDetailsDto MapToDetailsDto(Zadaci z) => new()
        {
            Id = z.Id,
            ProjekatId = z.ProjekatId,
            Naslov = z.Naslov,
            Opis = z.Opis,
            Status = z.Status,
            Prioritet = z.Prioritet,
            TipZadatka = z.TipZadatka,
            Rok = z.Rok,
            DatumKreiranja = z.DatumKreiranja,
            KreiraoKorisnikId = z.KreiraoKorisnikId,
            KreiraoKorisnikIme = $"{z.KreiraoKorisnik.Ime} {z.KreiraoKorisnik.Prezime}",
            DodeljenKorisnikuId = z.DodeljenKorisnikuId,
            DodeljenKorisnikuIme = z.DodeljenKorisniku != null
                ? $"{z.DodeljenKorisniku.Ime} {z.DodeljenKorisniku.Prezime}"
                : null
        };

        // ── Validators ───────────────────────────────────────────

        private static string? ValidateZadatak(string naslov, string status, string prioritet, string? opis, string? tipZadatka)
        {
            if (string.IsNullOrWhiteSpace(naslov))
                return "Naslov zadatka je obavezan.";
            if (naslov.Length > 50)
                return "Naslov zadatka ne sme imati vise od 50 karaktera.";
            if (string.IsNullOrWhiteSpace(status))
                return "Status zadatka je obavezan.";
            if (status.Length > 50)
                return "Status zadatka ne sme imati vise od 50 karaktera.";
            if (string.IsNullOrWhiteSpace(prioritet))
                return "Prioritet zadatka je obavezan.";
            if (prioritet.Length > 50)
                return "Prioritet zadatka ne sme imati vise od 50 karaktera.";
            if (opis != null && opis.Length > 150)
                return "Opis ne sme imati vise od 150 karaktera.";
            if (tipZadatka != null && tipZadatka.Length > 30)
                return "Tip zadatka ne sme imati vise od 30 karaktera.";
            return null;
        }

        private static string? ValidateRokUnutarProjekta(DateOnly? rok, DateTime projekatDatumPocetka, DateOnly? projekatRok)
        {
            if (!rok.HasValue)
                return null;

            if (rok.Value < DateOnly.FromDateTime(projekatDatumPocetka.Date))
                return "Rok zadatka ne moze biti pre datuma pocetka projekta.";

            if (projekatRok.HasValue && rok.Value > projekatRok.Value)
                return "Rok zadatka ne moze biti posle roka projekta.";

            return null;
        }

        private static string? ValidateDodeljenKorisnik(Guid? dodeljenKorisnikuId, List<Korisnici> clanoviProjekta)
        {
            if (!dodeljenKorisnikuId.HasValue)
                return null;

            return clanoviProjekta.Any(k => k.Id == dodeljenKorisnikuId.Value)
                ? null
                : "Izabrani korisnik nije clan projekta.";
        }

        private static string? CleanText(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
