using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Pets
{
    public class EditModel : PageModel
    {
        private readonly IPetService _petService;
        private readonly IUserService _userService;

        public EditModel(IPetService petService, IUserService userService)
        {
            _petService = petService;
            _userService = userService;
        }

        [BindProperty]
        public PetEditModel Pet { get; set; } = new PetEditModel();

        public SelectList? OwnerSelectList { get; set; }
        public Pet? CurrentPet { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                // Get the pet with owner details
                CurrentPet = await _petService.GetPetWithOwnerAsync(id);
                if (CurrentPet == null)
                {
                    TempData["ErrorMessage"] = "Pet not found.";
                    return RedirectToPage("./Index");
                }

                // Check authorization
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (!userId.HasValue)
                {
                    return RedirectToPage("/Account/Login");
                }

                // Customers can only edit their own pets
                if (userRole == "Customer" && CurrentPet.OwnerId != userId.Value)
                {
                    TempData["ErrorMessage"] = "You can only edit your own pets.";
                    return RedirectToPage("./Index");
                }

                // Load data for form
                await LoadDataAsync();

                // Populate the form model
                Pet = new PetEditModel
                {
                    Id = CurrentPet.Id,
                    OwnerId = CurrentPet.OwnerId,
                    Name = CurrentPet.Name,
                    Species = CurrentPet.Species,
                    Breed = CurrentPet.Breed,
                    Gender = CurrentPet.Gender,
                    BirthDate = CurrentPet.BirthDate,
                    Weight = CurrentPet.Weight,
                    Color = CurrentPet.Color,
                    MicrochipId = CurrentPet.MicrochipId,
                    MedicalNotes = CurrentPet.MedicalNotes,
                    IsActive = CurrentPet.IsActive
                };

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pet for edit: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading pet information.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            // Load data for dropdowns in case of validation errors
            await LoadDataAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Load current pet data for authorization check and preserving original data
                CurrentPet = await _petService.GetPetWithOwnerAsync(id);
                if (CurrentPet == null)
                {
                    TempData["ErrorMessage"] = "Pet not found.";
                    return RedirectToPage("./Index");
                }

                var userId = SessionHelper.GetUserId(HttpContext.Session);
                if (!userId.HasValue)
                {
                    ModelState.AddModelError("", "Unable to verify user session.");
                    return Page();
                }

                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                // Authorization check
                if (userRole == "Customer" && CurrentPet.OwnerId != userId.Value)
                {
                    ModelState.AddModelError("", "You can only edit your own pets.");
                    return Page();
                }

                // Update the pet entity
                var updatedPet = new Pet
                {
                    Id = id,
                    OwnerId = Pet.OwnerId,
                    Name = Pet.Name,
                    Species = Pet.Species,
                    Breed = Pet.Breed,
                    Gender = Pet.Gender,
                    BirthDate = Pet.BirthDate,
                    Weight = Pet.Weight,
                    Color = Pet.Color,
                    MicrochipId = Pet.MicrochipId,
                    MedicalNotes = Pet.MedicalNotes,
                    IsActive = Pet.IsActive,
                    CreatedDate = CurrentPet.CreatedDate // Preserve original creation date
                };

                await _petService.UpdatePetAsync(updatedPet);
                TempData["SuccessMessage"] = $"Pet '{Pet.Name}' has been updated successfully.";
                return RedirectToPage("./Index");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating pet: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the pet. Please try again.");
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var userId = SessionHelper.GetUserId(HttpContext.Session);
                var userRole = SessionHelper.GetUserRole(HttpContext.Session);

                if (userId.HasValue)
                {
                    // Load owners based on user role
                    IEnumerable<User> owners;
                    if (userRole == "Customer")
                    {
                        // Customers can only see themselves in the dropdown
                        var currentUser = await _userService.GetUserByIdAsync(userId.Value);
                        owners = currentUser != null ? new List<User> { currentUser } : new List<User>();
                    }
                    else
                    {
                        // Admin/Staff can see all customers
                        owners = await _userService.GetUsersByRoleAsync("Customer");
                    }
                    OwnerSelectList = new SelectList(owners, "Id", "FullName");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                OwnerSelectList = new SelectList(new List<User>(), "Id", "FullName");
            }
        }
    }

    public class PetEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select an owner")]
        [Display(Name = "Owner")]
        public int OwnerId { get; set; }

        [Required(ErrorMessage = "Pet name is required")]
        [StringLength(100, ErrorMessage = "Pet name cannot exceed 100 characters")]
        [Display(Name = "Pet Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Species is required")]
        [StringLength(50, ErrorMessage = "Species cannot exceed 50 characters")]
        [Display(Name = "Species")]
        public string Species { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Breed cannot exceed 100 characters")]
        [Display(Name = "Breed")]
        public string? Breed { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Range(0.1, 999.9, ErrorMessage = "Weight must be between 0.1 and 999.9 kg")]
        [Display(Name = "Weight (kg)")]
        public decimal? Weight { get; set; }

        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
        [Display(Name = "Color")]
        public string? Color { get; set; }

        [StringLength(100, ErrorMessage = "Microchip ID cannot exceed 100 characters")]
        [Display(Name = "Microchip ID")]
        public string? MicrochipId { get; set; }

        [StringLength(1000, ErrorMessage = "Medical notes cannot exceed 1000 characters")]
        [Display(Name = "Medical Notes")]
        public string? MedicalNotes { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}