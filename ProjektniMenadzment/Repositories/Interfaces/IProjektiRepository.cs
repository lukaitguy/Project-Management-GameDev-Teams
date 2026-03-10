using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektniMenadzment.Models.Domain;
using ProjektniMenadzment.Models.ViewModels;

namespace ProjektniMenadzment.Repositories.Interfaces
{
    public interface IProjektiRepository
    {
        Task<IEnumerable<Projekti>> GetAllAsync();
        Task<Projekti> GetByIdAsync(Guid id);
        Task<PregledProjektaViewModel?> GetDetailsByIdAsync(Guid id);
        Task<Projekti> AddAsync(Projekti projekat);
        Task<Projekti> UpdateAsync(Projekti projekat);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Projekti>> GetByKorisnikIdAsync(Guid korisnikId);
        Task<List<SelectListItem>> GetSelectOptionsAsync();
    }
}
