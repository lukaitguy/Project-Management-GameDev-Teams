using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;
using ProjektniMenadzment.Repositories;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Controllers
{
    public class ZadaciController : Controller
    {
        private readonly IZadaciRepository _zadaciRepository;
        private readonly IProjektiRepository _projektiRepository;
        private readonly IClanoviProjektaRepository _clanoviProjektaRepository;
        private readonly IKorisniciRepository _korisniciRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IZadaciKomentarRepository _komentar;

        public ZadaciController(IZadaciRepository zadaciRepository, IProjektiRepository projektiRepository, IClanoviProjektaRepository clanoviProjektaRepository, IKorisniciRepository korisniciRepository, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IZadaciKomentarRepository komentar)
        {
            _zadaciRepository = zadaciRepository;
            _projektiRepository = projektiRepository;
            _clanoviProjektaRepository = clanoviProjektaRepository;
            _korisniciRepository = korisniciRepository;
            _userManager = userManager;
            _komentar = komentar;
            _signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(Guid projekatId, string? status, bool? dodeljenFilter)
        {
            var zadaci = await _zadaciRepository.GetByProjekatIdAsync(projekatId);

            if (!string.IsNullOrEmpty(status))
            {
                zadaci = zadaci.Where(z => z.Status == status).ToList();
            }
            if (dodeljenFilter == true)
            {
                zadaci = zadaci.Where(z => z.DodeljenKorisnikuId != null).ToList();
            }
            else if (dodeljenFilter == false)
            {
                zadaci = zadaci.Where(z => z.DodeljenKorisnikuId == null).ToList();
            }

            var viewModel = zadaci.Select(z => new ZadaciViewModel
            {
                Id = z.Id,
                Naslov = z.Naslov,
                Opis = z.Opis,
                Status = z.Status,
                Prioritet = z.Prioritet,
                Rok = z.Rok != null ? z.Rok.Value : null,
                DatumKreiranja = z.DatumKreiranja,
                DodeljenKorisnikuIme = z.DodeljenKorisniku != null
                ? $"{z.DodeljenKorisniku.Ime} {z.DodeljenKorisniku.Prezime}"
                : "Nije dodeljen"
            }).ToList();

            var projekat = await _projektiRepository.GetByIdAsync(projekatId);

            string nazivProjekta = projekat.Naziv;

            ViewBag.ProjekatId = projekatId;
            ViewBag.NazivProjekta = nazivProjekta;
            ViewBag.Status = status;
            ViewBag.SamoDodeljeni = dodeljenFilter == true;
            ViewBag.NeDodeljeni = dodeljenFilter == false;

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var zadatak = await _zadaciRepository.GetByIdAsync(id);
            if (zadatak == null)
                return NotFound();

            var komentari = await _komentar.GetAllByIdAsync(id);
            var komentariViewModel = new List<ZadaciKomentariViewModel>();
            foreach(var k in komentari)
            {
                var authId = _userManager.GetUserId(User);
                var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(authId);
                komentariViewModel.Add(new ZadaciKomentariViewModel
                {
                    Komentar = k.Sadrzaj,
                    DatumKreiranja = k.DatumKreiranja,
                    Korisnik = korisnik.Ime + " " + korisnik.Prezime,
                });
            }

            var model = new ZadaciViewModel
            {
                Id = zadatak.Id,
                Naslov = zadatak.Naslov,
                Opis = zadatak.Opis,
                Status = zadatak.Status,
                Prioritet = zadatak.Prioritet,
                Rok = zadatak.Rok,
                DatumKreiranja = zadatak.DatumKreiranja,
                DodeljenKorisnikuIme = zadatak.DodeljenKorisniku != null
                    ? $"{zadatak.DodeljenKorisniku.Ime} {zadatak.DodeljenKorisniku.Prezime}"
                    : "Nije dodeljen",
                Komentari = komentariViewModel,
            };
            ViewBag.ProjekatId = zadatak.ProjekatId;

            return View(model);
        }

        [Authorize(Roles = "Admin, ProjektniMenadzer")]
        [HttpGet]
        public IActionResult Create(Guid projekatId)
        {
            var zadatak = new CreateZadatakRequest
            {
                ProjekatId = projekatId,               
            };
            return View(zadatak);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateZadatakRequest zadatak)
        {
            if (!ModelState.IsValid)
                return View(zadatak);

            var createZadatak = new Zadaci
            {
                Id = Guid.NewGuid(),
                ProjekatId = zadatak.ProjekatId,
                Naslov = zadatak.Naslov,
                Opis = zadatak.Opis,
                Rok = zadatak.Rok,
                Prioritet = zadatak.Prioritet,
                DodeljenKorisnikuId = null,
                DatumKreiranja = DateTime.UtcNow,
                Status = "Nije zapocet",
                KreiraoKorisnikId = Guid.Parse("a75c8586-e4db-4c81-9802-0b96e286367a")
            };

            await _zadaciRepository.AddAsync(createZadatak);
            return RedirectToAction("Index", new { projekatId = zadatak.ProjekatId });
        }

        [Authorize(Roles = "Admin, ProjektniMenadzer")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var zad = await _zadaciRepository.GetByIdAsync(id);
            if (zad == null) return NotFound();

            var clanovi = await _clanoviProjektaRepository
                .GetByProjekatIdAsync(zad.ProjekatId);

            var items = clanovi
                .Where(c => c.Korisnik != null)
                .Select(c => new SelectListItem
                {
                    Value = c.KorisnikId.ToString(),
                    Text = $"{c.Korisnik!.Ime} {c.Korisnik!.Prezime}"
                })
                .ToList();

            var model = new CreateZadatakRequest
            {
                Id = zad.Id,
                ProjekatId = zad.ProjekatId,
                Naslov = zad.Naslov,
                Opis = zad.Opis,
                Prioritet = zad.Prioritet,
                Rok = zad.Rok,
                DodeljenKorisnikuId = zad.DodeljenKorisnikuId,
                Korisnici = items
            };

            ViewBag.ZadatakId = zad.Id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CreateZadatakRequest model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                // ponovo popuni dropdown
                var clanovi = await _clanoviProjektaRepository.GetByProjekatIdAsync(model.ProjekatId);
                model.Korisnici = clanovi
                    .Where(c => c.Korisnik != null)
                    .Select(c => new SelectListItem
                    {
                        Value = c.KorisnikId.ToString(),
                        Text = $"{c.Korisnik!.Ime} {c.Korisnik!.Prezime}"
                    })
                    .ToList();
                ViewBag.ZadatakId = id;
                return View(model);
            }

            var zad = await _zadaciRepository.GetByIdAsync(id);
            if (zad == null) return NotFound();

            zad.Naslov = model.Naslov;
            zad.Opis = model.Opis;
            zad.Prioritet = model.Prioritet;
            zad.Rok = model.Rok;
            zad.DodeljenKorisnikuId = model.DodeljenKorisnikuId;

            await _zadaciRepository.UpdateAsync(zad);

            return RedirectToAction("Index", new { projekatId = model.ProjekatId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var zadatak = await _zadaciRepository.GetByIdAsync(id);
            if (zadatak == null)
                return NotFound();

            var projekatId = zadatak.ProjekatId;

            await _zadaciRepository.DeleteAsync(id);

            return RedirectToAction("Index", new { projekatId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid zadatakId, string noviStatus)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Unauthorized();

            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUser.Id);

            var isAdmin = await _userManager.IsInRoleAsync(identityUser, "Admin");
            var isMenadzer = await _userManager.IsInRoleAsync(identityUser, "ProjektniMenadzer");
            var canManageAll = isAdmin || isMenadzer;

            var uspeh = await _zadaciRepository.UpdateStatusAsync(zadatakId, korisnik?.Id, noviStatus, canManageAll);
            if (!uspeh)
            {
                TempData["Error"] = "Nije moguce promeniti status zadatka.";
            }
            else
            {
                TempData["Success"] = "Status zadatka je uspesno promenjen.";
            }
            return RedirectToAction("Details", new { id = zadatakId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Claim(Guid zadatakId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Forbid();

            var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(identityUser.Id);
            if (korisnik == null)
            {
                TempData["Error"] = "Nalog nije povezan sa app korisnikom.";
                return RedirectToAction("Index", "Projekti");
            }

            var uspeh = await _zadaciRepository.DodeliZadatakAsync(zadatakId, korisnik.Id);
            if (!uspeh)
            {
                TempData["Error"] = "Nije moguce preuzeti zadatak.";
            }
            else
            {
                TempData["Success"] = "Zadatak je uspesno preuzet.";
            }

            var z = await _zadaciRepository.GetByIdAsync(zadatakId);
            return RedirectToAction("Index", new { projekatId = z.ProjekatId });
        }

        [HttpPost]
        public async Task<IActionResult> Details(ZadaciViewModel zadatak)
        {
           
            if (_signInManager.IsSignedIn(User))
            {
                var authId = _userManager.GetUserId(User);
                var korisnik = await _korisniciRepository.GetByIdentityUserIdAsync(authId);
                var domainModel = new KomentariZadatak
                {
                    Id = Guid.NewGuid(),
                    Sadrzaj = zadatak.Komentar,
                    KorisnikId = korisnik.Id,
                    ZadatakId = zadatak.Id,
                    DatumKreiranja = DateTime.UtcNow,
                };
                await _komentar.AddAsync(domainModel);
                return RedirectToAction("Details", new { id = zadatak.Id });
            }

            return View();
        }

    }
}
