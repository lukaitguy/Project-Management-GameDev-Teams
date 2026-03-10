using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;
using ProjektniMenadzment.Repositories.Interfaces;

namespace ProjektniMenadzment.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IKorisniciRepository _korisniciRepository;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IKorisniciRepository korisniciRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _korisniciRepository = korisniciRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var identityUser = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var identityResult = await userManager.CreateAsync(identityUser, model.Password);

            if (identityResult.Succeeded)
            {
                var korisnik = new Korisnici
                {
                    Id = Guid.NewGuid(),
                    Ime = model.Ime,
                    Prezime = model.Prezime,
                    BrojTelefona = model.BrojTelefona,
                    Email = model.Email,
                    IdentityUserId = identityUser.Id,
                    DatumKreiranja = DateTime.Now,
                };
                
                var roleIdentityResult = await userManager.AddToRoleAsync(identityUser, "Korisnik");

                try
                {
                    await _korisniciRepository.AddAsync(korisnik);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Greška prilikom dodavanja korisnika: {ex.Message}");
                    return View(model);
                }


                if (roleIdentityResult.Succeeded)
                {
                    return RedirectToAction("Register");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var signInResult = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

            if (signInResult != null && signInResult.Succeeded)
            {
                return RedirectToAction("Index", "Projekti");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
