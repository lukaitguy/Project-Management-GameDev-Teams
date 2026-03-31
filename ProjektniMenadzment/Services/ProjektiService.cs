using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Services
{
    public class ProjektiService : IProjektiService
    {
        private readonly IProjektiRepository _projektiRepository;
        private readonly IZanroviRepository _zanroviRepository;
        private readonly IKorisniciRepository _korisniciRepository;

        public ProjektiService(IProjektiRepository projektiRepository, IZanroviRepository zanroviRepository, IKorisniciRepository korisniciRepository)
        {
            _projektiRepository = projektiRepository;
            _zanroviRepository = zanroviRepository;
            _korisniciRepository = korisniciRepository;
        }

        public async Task<ServiceResult<Guid>> CreateAsync(CreateProjekatDto dto)
        {
            //if (string.IsNullOrWhiteSpace(dto.Naziv))
            //{
            //    return ServiceResult<Guid>.Fail("Naziv projekta je obavezan.");
            //}

            //if (string.IsNullOrWhiteSpace(dto.Status))
            //{
            //    return ServiceResult<Guid>.Fail("Status projekta je obavezan.");
            //}

            //var zanrovi = new List<Zanrovi>();

            throw new NotImplementedException();

        }

        public async Task<ServiceResult<ProjekatDetailsDto>> GetByIdAsync(Guid id)
        {
            var projekat = await _projektiRepository.GetByIdAsync(id);

            if(projekat == null)
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

            if(korisnik == null)
            {
                return ServiceResult<IEnumerable<ProjekatListDto>>.Fail("Korisnik ne postoji.");
            }

            var projekti = await _projektiRepository.GetByKorisnikIdAsync(korisnik.Id);

            var result = projekti.Select(p => new ProjekatListDto
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

            return ServiceResult<IEnumerable<ProjekatListDto>>.Ok(result);
        }

        public Task UpdateAsync(UpdateProjekatDto req)
        {
            throw new NotImplementedException();
        }
    }
}
