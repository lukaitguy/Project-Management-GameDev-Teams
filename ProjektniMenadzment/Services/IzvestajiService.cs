using ProjektniMenadzment.Models.DTOs;
using ProjektniMenadzment.Repositories.Interfaces;
using ProjektniMenadzment.Services.Interfaces;

namespace ProjektniMenadzment.Services
{
    public class IzvestajiService : IIzvestajiService
    {
        private readonly IProjektiRepository _projektiRepo;
        private readonly IZadaciRepository _zadaciRepo;
        private readonly IClanoviProjektaRepository _clanoviRepo;
        private readonly IResursiRepository _resursiRepo;
        private readonly IKorisniciRepository _korisniciRepo;

        public IzvestajiService(
            IProjektiRepository projektiRepo,
            IZadaciRepository zadaciRepo,
            IClanoviProjektaRepository clanoviRepo,
            IResursiRepository resursiRepo,
            IKorisniciRepository korisniciRepo)
        {
            _projektiRepo = projektiRepo;
            _zadaciRepo = zadaciRepo;
            _clanoviRepo = clanoviRepo;
            _resursiRepo = resursiRepo;
            _korisniciRepo = korisniciRepo;
        }

        public async Task<ServiceResult<ProjekatIzvestajDto>> GetProjekatIzvestajAsync(
            Guid projekatId, string identityUserId, bool isAdmin)
        {
            var projekat = await _projektiRepo.GetByIdAsync(projekatId);
            if (projekat == null)
                return ServiceResult<ProjekatIzvestajDto>.Fail("Projekat nije pronadjen.");

            if (!isAdmin)
            {
                var korisnik = await _korisniciRepo.GetByIdentityUserIdAsync(identityUserId);
                if (korisnik == null)
                    return ServiceResult<ProjekatIzvestajDto>.Fail("Korisnik nije pronadjen.");

                var clanovi = await _clanoviRepo.GetByProjekatIdAsync(projekatId);
                if (!clanovi.Any(c => c.KorisnikId == korisnik.Id))
                    return ServiceResult<ProjekatIzvestajDto>.Fail("Nemate pristup izvestaju ovog projekta.");
            }

            var zadaci = (await _zadaciRepo.GetByProjekatIdAsync(projekatId)).ToList();
            var clanProjekta = await _clanoviRepo.GetByProjekatIdAsync(projekatId);
            var resursi = (await _resursiRepo.GetByProjekatIdAsync(projekatId)).ToList();

            var danas = DateOnly.FromDateTime(DateTime.UtcNow);

            var zavrsenihZadataka   = zadaci.Count(z => z.Status == "Zavrsen");
            var uTokuZadataka       = zadaci.Count(z => z.Status == "U toku");
            var nijeZapocetih       = zadaci.Count(z => z.Status == "Nije zapocet");
            var pauziranihZadataka  = zadaci.Count(z => z.Status == "Pauziran");
            var otkazanihZadataka   = zadaci.Count(z => z.Status == "Otkazan");
            var zakasnelih          = zadaci.Count(z => z.Rok.HasValue && z.Rok.Value < danas && z.Status != "Zavrsen");

            var postotakZavrsenosti = zadaci.Count > 0
                ? Math.Round((double)zavrsenihZadataka / zadaci.Count * 100, 1)
                : 0;

            var clanoviIzvestaj = clanProjekta.Select(cp =>
            {
                var imeIPrezime = $"{cp.Korisnik.Ime} {cp.Korisnik.Prezime}".Trim();
                var korisnikZadaci = zadaci.Where(z => z.DodeljenKorisnikuId == cp.KorisnikId).ToList();

                return new ClanIzvestajDto
                {
                    ImeIPrezime = imeIPrezime,
                    Uloga = cp.Uloga,
                    UkupnoZadataka = korisnikZadaci.Count,
                    ZavrsenihZadataka = korisnikZadaci.Count(z => z.Status == "Zavrsen"),
                    AktivnihZadataka = korisnikZadaci.Count(z => z.Status == "U toku")
                };
            }).OrderByDescending(c => c.UkupnoZadataka).ToList();

            var resursiIzvestaj = resursi.Select(r => new ResursIzvestajDto
            {
                Naziv = r.Naziv,
                Tip = r.Tip,
                Cena = r.Cena,
                DodeljenKorisnikuIme = r.DodeljenKorisnikuNavigation != null
                    ? $"{r.DodeljenKorisnikuNavigation.Ime} {r.DodeljenKorisnikuNavigation.Prezime}".Trim()
                    : null
            }).ToList();

            var ukupniTroskovi = resursi.Sum(r => r.Cena ?? 0);
            var preostaliButzet = projekat.Budzet.HasValue
                ? projekat.Budzet.Value - ukupniTroskovi
                : (decimal?)null;

            var dto = new ProjekatIzvestajDto
            {
                Id = projekat.Id,
                Naziv = projekat.Naziv,
                Status = projekat.Status,
                FazaRazvoja = projekat.FazaRazvoja,
                Platforma = projekat.Platforma,
                Engine = projekat.Engine,
                DatumPocetka = projekat.DatumPocetka,
                Rok = projekat.Rok,
                Budzet = projekat.Budzet,

                UkupnoZadataka = zadaci.Count,
                ZavrsenihZadataka = zavrsenihZadataka,
                UTokuZadataka = uTokuZadataka,
                NijeZapocetihZadataka = nijeZapocetih,
                PauziranihZadataka = pauziranihZadataka,
                OtkazanihZadataka = otkazanihZadataka,
                ZakasnelihZadataka = zakasnelih,
                PostotakZavrsenosti = postotakZavrsenosti,

                Clanovi = clanoviIzvestaj,

                UkupniTroskoviResursa = ukupniTroskovi,
                PreostaliButzet = preostaliButzet,
                Resursi = resursiIzvestaj
            };

            return ServiceResult<ProjekatIzvestajDto>.Ok(dto);
        }
    }
}
