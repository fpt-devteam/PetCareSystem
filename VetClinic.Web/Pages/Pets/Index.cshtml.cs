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

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SpeciesFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        // Pagination properties
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                UserRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "Customer";
                var userId = SessionHelper.GetUserId(HttpContext.Session);

                if (!userId.HasValue)
                {
                    Pets = new List<Pet>();
                    return Page();
                }

                // Load pets based on user role
                IEnumerable<Pet> allPets;
                if (UserRole == "Customer")
                {
                    // Customers see only their own pets
                    allPets = await _petService.GetPetsByOwnerIdAsync(userId.Value);
                }
                else
                {
                    // Admin, Manager, Doctor, Staff see all pets
                    allPets = await _petService.GetAllPetsAsync();
                }

                // Apply filters
                Pets = FilterPets(allPets);

                // Apply pagination
                var totalCount = Pets.Count();
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
                CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

                Pets = Pets.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pets: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading pets. Please try again.";
                Pets = new List<Pet>();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int petId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (!userId.HasValue)
                {
                    TempData["ErrorMessage"] = "Unable to verify user session.";
                    return RedirectToPage();
                }

                // Get the pet to check authorization
                var pet = await _petService.GetPetByIdAsync(petId);
                if (pet == null)
                {
                    TempData["ErrorMessage"] = "Pet not found.";
                    return RedirectToPage();
                }

                // Authorization check
                if (userRole == "Customer" && pet.OwnerId != userId.Value)
                {
                    TempData["ErrorMessage"] = "You can only delete your own pets.";
                    return RedirectToPage();
                }

                var success = await _petService.DeletePetAsync(petId);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Pet '{pet.Name}' has been deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete pet. Please try again.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting pet: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting pet. Please try again.";
            }

            return RedirectToPage();
        }

        private IEnumerable<Pet> FilterPets(IEnumerable<Pet> pets)
        {
            // Apply search filter
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                pets = pets.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    (p.Breed?.ToLower().Contains(searchLower) ?? false) ||
                    (p.Owner?.FullName.ToLower().Contains(searchLower) ?? false));
            }

            // Apply species filter
            if (!string.IsNullOrEmpty(SpeciesFilter))
            {
                pets = pets.Where(p => p.Species == SpeciesFilter);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(StatusFilter))
            {
                var isActive = bool.Parse(StatusFilter);
                pets = pets.Where(p => p.IsActive == isActive);
            }

            return pets.OrderBy(p => p.Name);
        }

        public int CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue) return 0;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;
            if (birthDate.Value.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}