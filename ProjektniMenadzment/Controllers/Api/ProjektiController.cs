using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjektiController : ControllerBase
    {
        private readonly IProjektiRepository _projektiRepository;
        private readonly IKorisniciRepository _korisniciRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjektiController(
            IProjektiRepository projektiRepository,
            IKorisniciRepository korisniciRepository,
            UserManager<IdentityUser> userManager)
        {
            _projektiRepository = projektiRepository;
            _userManager = userManager;
            _korisniciRepository = korisniciRepository;
        }

        [HttpGet("moji")]
        public async Task<IActionResult> GetMojiProjekti()
        {
            var identityUser = await _userManager.GetUserAsync(User);

            if(identityUser == null)
            {
                return Unauthorized(new { message = "Korisnik nije prijavljen." });
            }

            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUser.Id);

            if(korisnik == null)
            {
                return NotFound(new { message = "Korisnik ne postoji u bazi." });
            }

            var projekti = await _projektiRepository.GetByKorisnikIdAsync(korisnik.Id);

            var rezultat = projekti.Select(p => new ProjekatListDto
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
            }).ToList();

            return Ok(rezultat);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjekatById(Guid id)
        {
            var projekat = await _projektiRepository.GetByIdAsync(id);

            if (projekat == null)
            {
                return NotFound(new { message = "Projekat nije pronađen." });
            }

            var dto = new ProjekatDetailsDto
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

            return Ok(dto);
        }

    }
}
