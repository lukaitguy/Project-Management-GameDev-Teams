using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Services
{
    public class ProjektiService : IProjektiService
    {
        private readonly IProjektiRepository _projektiRepository;
        private readonly IKorisniciRepository _korisniciRepository;
        private readonly IClanoviProjektaRepository _clanoviProjektaRepository;
        private readonly IZanroviRepository _zanroviRepository;

        public ProjektiService(
            IProjektiRepository projektiRepository,
            IKorisniciRepository korisniciRepository,
            IClanoviProjektaRepository clanoviProjektaRepository,
            IZanroviRepository zanroviRepository)
        {
            _projektiRepository = projektiRepository;
            _korisniciRepository = korisniciRepository;
            _clanoviProjektaRepository = clanoviProjektaRepository;
            _zanroviRepository = zanroviRepository;
        }

        public async Task<ServiceResult<Guid>> CreateAsync(CreateProjekatDto dto, string identityUserId)
        {
            var validationMessage = ValidateProject(
                dto.Naziv, dto.Status, dto.DatumPocetka, dto.Rok, dto.Budzet,
                dto.Opis, dto.VerzijaIgre, dto.Engine, dto.Platforma, dto.FazaRazvoja);
            if (validationMessage != null)
            {
                return ServiceResult<Guid>.Fail(validationMessage);
            }

            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUserId);
            if (korisnik == null)
            {
                return ServiceResult<Guid>.Fail("Aplikacioni profil korisnika nije pronadjen.");
            }

            var projekat = new Projekti
            {
                Id = Guid.NewGuid(),
                Naziv = dto.Naziv.Trim(),
                Opis = CleanText(dto.Opis),
                Status = dto.Status.Trim(),
                Budzet = dto.Budzet,
                DatumPocetka = dto.DatumPocetka,
                Rok = dto.Rok,
                KreiraoKorisnikId = korisnik.Id,
                DatumKreiranja = DateTime.UtcNow,
                VerzijaIgre = CleanText(dto.VerzijaIgre),
                Engine = CleanText(dto.Engine),
                Platforma = CleanText(dto.Platforma),
                FazaRazvoja = CleanText(dto.FazaRazvoja)
            };

            if (dto.SelectedZanrIds.Count > 0)
            {
                var zanrovi = await _zanroviRepository.GetByIdsAsync(dto.SelectedZanrIds);
                foreach (var zanr in zanrovi)
                {
                    projekat.Zanrs.Add(zanr);
                }
            }

            try
            {
                await _projektiRepository.AddAsync(projekat);
                await _clanoviProjektaRepository.AddAsync(new ClanoviProjektum
                {
                    ProjekatId = projekat.Id,
                    KorisnikId = korisnik.Id,
                    Uloga = "Administrator"
                });
            }
            catch (DbUpdateException)
            {
                return ServiceResult<Guid>.Fail("Doslo je do greske pri cuvanju projekta.");
            }

            return ServiceResult<Guid>.Ok(projekat.Id);
        }

        public async Task<ServiceResult<IEnumerable<ProjekatListDto>>> GetAllAsync()
        {
            var projekti = await _projektiRepository.GetAllAsync();
            return ServiceResult<IEnumerable<ProjekatListDto>>.Ok(MapProjects(projekti));
        }

        public async Task<ServiceResult<ProjekatDetailsDto>> GetByIdAsync(Guid id)
        {
            var projekat = await _projektiRepository.GetByIdAsync(id);

            if (projekat == null)
            {
                return ServiceResult<ProjekatDetailsDto>.Fail("Projekat nije pronadjen.");
            }

            var result = new ProjekatDetailsDto
            {
                Id = projekat.Id,
                Naziv = projekat.Naziv,
                Opis = projekat.Opis,
                Status = projekat.Status,
                Budzet = projekat.Budzet,
                DatumPocetka = projekat.DatumPocetka,
                Rok = projekat.Rok,
                VerzijaIgre = projekat.VerzijaIgre,
                Engine = projekat.Engine,
                Platforma = projekat.Platforma,
                FazaRazvoja = projekat.FazaRazvoja,
                DatumPoslednjegBuilda = projekat.DatumPoslednjegBuilda
            };

            return ServiceResult<ProjekatDetailsDto>.Ok(result);
        }

        public async Task<ServiceResult<IEnumerable<ProjekatListDto>>> GetMojiProjektiAsync(string identityUserId)
        {
            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUserId);

            if (korisnik == null)
            {
                return ServiceResult<IEnumerable<ProjekatListDto>>.Fail("Korisnik ne postoji.");
            }

            var projekti = await _projektiRepository.GetByKorisnikIdAsync(korisnik.Id);
            return ServiceResult<IEnumerable<ProjekatListDto>>.Ok(MapProjects(projekti));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(Guid id, UpdateProjekatDto dto)
        {
            var validationMessage = ValidateProject(
                dto.Naziv, dto.Status, dto.DatumPocetka, dto.Rok, dto.Budzet,
                dto.Opis, dto.VerzijaIgre, dto.Engine, dto.Platforma, dto.FazaRazvoja);
            if (validationMessage != null)
            {
                return ServiceResult<bool>.Fail(validationMessage);
            }

            var projekat = await _projektiRepository.GetByIdAsync(id);
            if (projekat == null)
            {
                return ServiceResult<bool>.Fail("Projekat nije pronadjen.");
            }

            projekat.Naziv = dto.Naziv.Trim();
            projekat.Opis = CleanText(dto.Opis);
            projekat.Status = dto.Status.Trim();
            projekat.Budzet = dto.Budzet;
            projekat.DatumPocetka = dto.DatumPocetka;
            projekat.Rok = dto.Rok;
            projekat.VerzijaIgre = CleanText(dto.VerzijaIgre);
            projekat.Engine = CleanText(dto.Engine);
            projekat.Platforma = CleanText(dto.Platforma);
            projekat.FazaRazvoja = CleanText(dto.FazaRazvoja);

            try
            {
                await _projektiRepository.UpdateAsync(projekat);
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Doslo je do greske pri izmeni projekta.");
            }

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var uspesno = await _projektiRepository.DeleteAsync(id);
                if (!uspesno)
                {
                    return ServiceResult<bool>.Fail("Projekat nije pronadjen.");
                }
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Projekat ne moze da se obrise dok postoje povezani podaci.");
            }

            return ServiceResult<bool>.Ok(true);
        }

        private static IEnumerable<ProjekatListDto> MapProjects(IEnumerable<Projekti> projekti)
        {
            return projekti.Select(p => new ProjekatListDto
            {
                Id = p.Id,
                Naziv = p.Naziv,
                Opis = p.Opis,
                Status = p.Status,
                Budzet = p.Budzet,
                DatumPocetka = p.DatumPocetka,
                Rok = p.Rok,
                VerzijaIgre = p.VerzijaIgre,
                Engine = p.Engine,
                Platforma = p.Platforma,
                FazaRazvoja = p.FazaRazvoja,
                DatumPoslednjegBuilda = p.DatumPoslednjegBuilda
            });
        }

        private static string? ValidateProject(
            string naziv, string status, DateTime datumPocetka, DateOnly? rok, decimal? budzet,
            string? opis, string? verzijaIgre, string? engine, string? platforma, string? fazaRazvoja)
        {
            if (string.IsNullOrWhiteSpace(naziv))
            {
                return "Naziv projekta je obavezan.";
            }

            if (naziv.Length > 50)
            {
                return "Naziv projekta ne sme imati vise od 50 karaktera.";
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                return "Status projekta je obavezan.";
            }

            if (status.Length > 50)
            {
                return "Status ne sme imati vise od 50 karaktera.";
            }

            if (datumPocetka == default)
            {
                return "Datum pocetka je obavezan.";
            }

            if (rok.HasValue && rok.Value < DateOnly.FromDateTime(datumPocetka.Date))
            {
                return "Rok ne moze biti pre datuma pocetka.";
            }

            if (budzet.HasValue && budzet.Value < 0)
            {
                return "Budzet ne moze biti negativan.";
            }

            if (opis != null && opis.Length > 150)
            {
                return "Opis ne sme imati vise od 150 karaktera.";
            }

            if (verzijaIgre != null && verzijaIgre.Length > 20)
            {
                return "Verzija igre ne sme imati vise od 20 karaktera.";
            }

            if (engine != null && engine.Length > 50)
            {
                return "Engine ne sme imati vise od 50 karaktera.";
            }

            if (platforma != null && platforma.Length > 100)
            {
                return "Platforma ne sme imati vise od 100 karaktera.";
            }

            if (fazaRazvoja != null && fazaRazvoja.Length > 30)
            {
                return "Faza razvoja ne sme imati vise od 30 karaktera.";
            }

            return null;
        }

        private static string? CleanText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
