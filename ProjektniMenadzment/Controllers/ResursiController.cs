using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Controllers
{
    public class ResursiController : Controller
    {
        private readonly IProjektiRepository _projektiRepository;
        private readonly IResursiRepository _resursiRepository;
        private readonly IKorisniciRepository _korisniciRepository;

        public ResursiController(IProjektiRepository projektiRepository, IResursiRepository resursiRepository, IKorisniciRepository korisniciRepository)
        {
            _projektiRepository = projektiRepository;
            _resursiRepository = resursiRepository;
            _korisniciRepository = korisniciRepository;
        }

        [Authorize(Roles = "Admin, ProjektniMenadzer")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var resursi = await _resursiRepository.GetAllAsync();

            if (resursi != null)
            {
                var viewModel = resursi.Select(r => new ResursiViewModel
                {
                    Id = r.Id,
                    Naziv = r.Naziv,
                    Tip = r.Tip,
                    Opis = r.Opis,
                    Cena = r.Cena,
                    DodeljenKorisniku = r.DodeljenKorisnikuNavigation != null ? $"{r.DodeljenKorisnikuNavigation.Ime} {r.DodeljenKorisnikuNavigation.Prezime}" : "Nije dodeljeno",
                    Projekat = r.Projekat != null ? r.Projekat.Naziv : "Nije dodeljeno",
                }).ToList();

                return View(viewModel);
            }
            else
            {
                return NotFound("Nema dostupnih resursa.");
            }
        }

        [Authorize(Roles = "Admin, ProjektniMenadzer")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var projekti = await _projektiRepository.GetSelectOptionsAsync();
            var korisnici = await _korisniciRepository.GetSelectOptionsAsync();

            var model = new CreateResursViewModel
            {
                Projekti = projekti,
                Korisnici = korisnici
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateResursViewModel resurs)
        {
            if (!ModelState.IsValid)
            {
                var projekti = await _projektiRepository.GetSelectOptionsAsync();
                var korisnici = await _korisniciRepository.GetSelectOptionsAsync();

                resurs.Projekti = projekti;

                resurs.Korisnici = korisnici;

                return View(resurs);
            }

            if (resurs != null)
            {
                var model = new Resursi
                {
                    Id = Guid.NewGuid(),
                    Naziv = resurs.Naziv,
                    Tip = resurs.Tip,
                    Opis = resurs.Opis,
                    Cena = resurs.Cena,
                    DatumKreiranja = DateTime.UtcNow,
                    DodeljenKorisniku = resurs.DodeljenKorisniku,
                    ProjekatId = resurs.ProjekatId,
                };
                await _resursiRepository.AddAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin, ProjektniMenadzer")]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var resurs = await _resursiRepository.GetByIdAsync(id);

            var model = new ResursiViewModel
            {
                Id = resurs.Id,
                Naziv = resurs.Naziv,
                Tip = resurs.Tip,
                Opis = resurs.Opis,
                Cena = resurs.Cena,
                Projekat = resurs.Projekat != null ? resurs.Projekat.Naziv : "Nije dodeljeno",
                DodeljenKorisniku = resurs.DodeljenKorisnikuNavigation != null ? $"{resurs.DodeljenKorisnikuNavigation.Ime} {resurs.DodeljenKorisnikuNavigation.Prezime}" : "Nije dodeljeno",
                DatumKreiranja = resurs.DatumKreiranja
            };

            if (model == null) return NotFound();
            return View(model);
        }

        [Authorize(Roles = "Admin, ProjektniMenadzer")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var resurs = await _resursiRepository.GetByIdAsync(id);

            var projekti = await _projektiRepository.GetSelectOptionsAsync();
            var korisnici = await _korisniciRepository.GetSelectOptionsAsync();

            var model = new CreateResursViewModel
            {
                Id = resurs.Id,
                Naziv = resurs.Naziv,
                Tip = resurs.Tip,
                Opis = resurs.Opis,
                Cena = resurs.Cena,
                DodeljenKorisniku = resurs.DodeljenKorisniku,
                ProjekatId = resurs.ProjekatId,
                Projekti = projekti,
                Korisnici = korisnici

            };

            ViewBag.Id = id;

            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateResursViewModel resurs)
        {
            if (!ModelState.IsValid)
            {
                var projekti = await _projektiRepository.GetSelectOptionsAsync();
                var korisnici = await _korisniciRepository.GetSelectOptionsAsync();

                return View(resurs);
            }
            var model = new Resursi
            {
                Id = resurs.Id,
                Naziv = resurs.Naziv,
                Tip = resurs.Tip,
                Opis = resurs.Opis,
                Cena = resurs.Cena,
                DatumKreiranja = DateTime.UtcNow,
                DodeljenKorisniku = resurs.DodeljenKorisniku,
                ProjekatId = resurs.ProjekatId,
            };

            await _resursiRepository.UpdateAsync(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resurs = await _resursiRepository.GetByIdAsync(id);
            if (resurs == null)
            {
                return NotFound();
            }
            await _resursiRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
