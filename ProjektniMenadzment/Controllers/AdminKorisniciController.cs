using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminKorisniciController : Controller
    {
        private readonly IAdminKorisniciRepository _adminKorisniciRepository;

        public AdminKorisniciController(IAdminKorisniciRepository adminKorisniciRepository)
        {
            _adminKorisniciRepository = adminKorisniciRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? role, bool? onlyLinked)
        {
            ViewBag.AllRoles = await _adminKorisniciRepository.GetAllRolesAsync();
            var model = await _adminKorisniciRepository.GetAllAsync(search, role, onlyLinked == true);
            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.OnlyLinked = onlyLinked == true;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.AllRoles = await _adminKorisniciRepository.GetAllRolesAsync();
            var vm = await _adminKorisniciRepository.GetByIdentityIdAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(KorisniciViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllRoles = await _adminKorisniciRepository.GetAllRolesAsync();
                return View(vm);
            }

            await _adminKorisniciRepository.UpdateAsync(vm);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            return View(new PromenaLozinkeViewModel { IdentityUserId = id });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(PromenaLozinkeViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            await _adminKorisniciRepository.ChangePasswordAsync(vm);
            return RedirectToAction(nameof(Edit), new { id = vm.IdentityUserId });
        }

        [HttpPost]
        public async Task<IActionResult> LinkAppUser(KorisniciViewModel vm)
        {
            await _adminKorisniciRepository.CreateAppUserAsync(vm);
            return RedirectToAction(nameof(Edit), new { id = vm.IdentityUserId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _adminKorisniciRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.AllRoles = await _adminKorisniciRepository.GetAllRolesAsync();
            return View(new KreirajKorisnikaViewModel { KreirajAppProfil = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(KreirajKorisnikaViewModel korisnik)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllRoles = await _adminKorisniciRepository.GetAllRolesAsync();
                return View(korisnik);
            }

            try
            {
                await _adminKorisniciRepository.CreateAsync(korisnik);
                TempData["Success"] = "Korisnik je uspešno kreiran.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.AllRoles = await _adminKorisniciRepository.GetAllRolesAsync();
                return View(korisnik);
            }
        }
    }
}
