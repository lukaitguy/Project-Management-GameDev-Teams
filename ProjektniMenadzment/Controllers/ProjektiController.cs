using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Controllers
{
    public class ProjektiController : Controller
    {
        private readonly IProjektiRepository _projektiRepository;
        private readonly IZanroviRepository _zanroviRepository;
        private readonly IKorisniciRepository _korisniciRepository;
        private readonly IClanoviProjektaRepository _clanoviProjektaRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjektiController(IProjektiRepository projektiRepository, IZanroviRepository zanroviRepository, IKorisniciRepository korisniciRepository, IClanoviProjektaRepository clanoviProjektaRepository, UserManager<IdentityUser> userManager)
        {
            _projektiRepository = projektiRepository;
            _zanroviRepository = zanroviRepository;
            _korisniciRepository = korisniciRepository;
            _clanoviProjektaRepository = clanoviProjektaRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUser.Id);

            if (User.IsInRole("Administrator") || User.IsInRole("ProjektniMenadzer"))
            {
                var projekti = await _projektiRepository.GetAllAsync();
                var viewModel = projekti.Select(p => new ProjekatViewModel
                {
                    Id = p.Id,
                    Naziv = p.Naziv,
                    Status = p.Status,
                    DatumPocetka = p.DatumPocetka,
                    Rok = p.Rok,
                    //KreiraoKorisnikIme = p.KreiraoKorisnik?.Ime ?? "Nepoznato"
                }).ToList();
                return View(viewModel);
            }
            else
            {
                var projekti = await _projektiRepository.GetByKorisnikIdAsync(korisnik.Id);

                var viewModel = projekti.Select(p => new ProjekatViewModel
                {
                    Id = p.Id,
                    Naziv = p.Naziv,
                    Status = p.Status,
                    DatumPocetka = p.DatumPocetka,
                    Rok = p.Rok,
                    //KreiraoKorisnikIme = p.KreiraoKorisnik?.Ime ?? "Nepoznato"
                }).ToList();

                return View(viewModel);
            }

        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, string? statusZadatka)
        {
            var model = await _projektiRepository.GetDetailsByIdAsync(id);
            if (model == null)
                return NotFound();

            ViewBag.StatusZadatka = statusZadatka;

            return View(model);
        }

        [Authorize(Roles = "Administrator, ProjektniMenadzer")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var zanrovi = await _zanroviRepository.GetAllAsync();

            var model = new CreateProjekatRequest
            {
                DatumPocetka = DateTime.Now,
                Zanrovi = zanrovi.Select(z => new SelectListItem
                {
                    Value = z.Id.ToString(),
                    Text = z.Naziv
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjekatRequest model)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUser.Id);

            if (!ModelState.IsValid)
            {
                var zanrovi = await _zanroviRepository.GetAllAsync();
                model.Zanrovi = zanrovi.Select(z => new SelectListItem
                {
                    Value = z.Id.ToString(),
                    Text = z.Naziv
                }).ToList();

                return View(model);
            }

            var projekat = new Projekti
            {
                Id = Guid.NewGuid(),
                Naziv = model.Naziv,
                Opis = model.Opis,
                Status = model.Status,
                Budzet = model.Budzet,
                DatumPocetka = model.DatumPocetka,
                Rok = model.Rok,
                KreiraoKorisnikId = korisnik.Id,
                DatumKreiranja = DateTime.Now,
            };

            await _projektiRepository.AddAsync(projekat);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator, ProjektniMenadzer")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var projekat = await _projektiRepository.GetByIdAsync(id);

            if (projekat == null)
                return NotFound();

            var zanrovi = await _zanroviRepository.GetAllAsync();

            var model = new EditProjekatRequest
            {
                Naziv = projekat.Naziv,
                Opis = projekat.Opis,
                Status = projekat.Status,
                Budzet = projekat.Budzet ?? 0,
                DatumPocetka = projekat.DatumPocetka,
                Rok = projekat.Rok
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProjekatRequest model)
        {
            if (!ModelState.IsValid)
            {
                var zanrovi = await _zanroviRepository.GetAllAsync();
                model.Zanrovi = zanrovi.Select(z => new SelectListItem
                {
                    Value = z.Id.ToString(),
                    Text = z.Naziv
                }).ToList();

                return View(model);
            }

            var projekat = new Projekti
            {
                Id = model.Id,
                Naziv = model.Naziv,
                Opis = model.Opis,
                Status = model.Status,
                Budzet = model.Budzet,
                DatumPocetka = model.DatumPocetka,
                Rok = model.Rok,
            };

            await _projektiRepository.UpdateAsync(projekat);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var uspesno = await _projektiRepository.DeleteAsync(id);
            if (!uspesno)
                return NotFound();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, ProjektniMenadzer")]
        [HttpGet]
        public async Task<IActionResult> AddMember(Guid id)
        {
            var projekat = await _projektiRepository.GetByIdAsync(id);

            if (projekat == null)
                return NotFound();

            var korisnici = await _korisniciRepository.GetAllAsync();

            var model = new AddMemberViewModel
            {
                ProjekatId = id,
                Korisnici = korisnici.Select(k => new SelectListItem
                {
                    Value = k.Id.ToString(),
                    Text = $"{k.Ime} {k.Prezime}"
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(AddMemberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var korisnici = await _korisniciRepository.GetAllAsync();
                model.Korisnici = korisnici.Select(k => new SelectListItem
                {
                    Value = k.Id.ToString(),
                    Text = $"{k.Ime} {k.Prezime}"
                }).ToList();
                return View(model);
            }

            var clan = new ClanoviProjektum
            {
                ProjekatId = model.ProjekatId,
                KorisnikId = model.KorisnikId,
                Uloga = model.Uloga
            };

            await _clanoviProjektaRepository.AddAsync(clan);

            return RedirectToAction("Details", new { id = model.ProjekatId });
        }


    }
}
