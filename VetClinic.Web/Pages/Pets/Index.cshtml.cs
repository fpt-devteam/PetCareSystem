using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Pets
{
    public class IndexModel : PageModel
    {
        private readonly IPetService _petService;

        public IndexModel(IPetService petService)
        {
            _petService = petService;
        }

        public IEnumerable<Pet> Pets { get; set; } = new List<Pet>();
        public string UserRole { get; set; } = string.Empty;
        public string SearchTerm { get; set; } = string.Empty;
        public string SpeciesFilter { get; set; } = string.Empty;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public async Task<IActionResult> OnGetAsync(string? searchTerm, string? speciesFilter, int page = 1)
        {
            // Check if user is authenticated
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            UserRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "Customer";
            SearchTerm = searchTerm ?? string.Empty;
            SpeciesFilter = speciesFilter ?? string.Empty;
            CurrentPage = page;

            var userId = SessionHelper.GetUserId(HttpContext.Session);
            if (!userId.HasValue)
            {
                Pets = new List<Pet>();
                return Page();
            }

            try
            {
                IEnumerable<Pet> allPets;

                // Load pets based on user role
                if (UserRole == "Customer")
                {
                    allPets = await _petService.GetPetsByOwnerIdAsync(userId.Value);
                }
                else
                {
                    allPets = await _petService.GetAllPetsAsync();
                }

                // Apply filters for non-customer roles
                if (UserRole != "Customer")
                {
                    if (!string.IsNullOrWhiteSpace(SearchTerm))
                    {
                        allPets = allPets.Where(p =>
                            p.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                            p.Species.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                            (p.Owner?.FullName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
                    }

                    if (!string.IsNullOrWhiteSpace(SpeciesFilter))
                    {
                        allPets = allPets.Where(p => p.Species.Equals(SpeciesFilter, StringComparison.OrdinalIgnoreCase));
                    }
                }

                // Pagination
                var totalPets = allPets.Count();
                TotalPages = (int)Math.Ceiling((double)totalPets / PageSize);

                Pets = allPets
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading pets. Please try again.";
                Console.WriteLine($"Error loading pets: {ex.Message}");
                Pets = new List<Pet>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userRole = SessionHelper.GetUserRole(HttpContext.Session);

            if (userRole != "Admin" && userRole != "Manager")
            {
                TempData["Error"] = "You are not authorized to delete pets.";
                return RedirectToPage();
            }

            try
            {
                await _petService.DeletePetAsync(id);
                TempData["Success"] = "Pet deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting pet: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}