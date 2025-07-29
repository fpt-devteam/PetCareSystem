using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace VetClinic.Web.Pages.Pets
{
    public class CreateModel : PageModel
    {
        private readonly IPetService _petService;
        private readonly IUserService _userService;

        public CreateModel(IPetService petService, IUserService userService)
        {
            _petService = petService;
            _userService = userService;
        }

        [BindProperty]
        public PetCreateModel Pet { get; set; } = new PetCreateModel();

        public SelectList? OwnerSelectList { get; set; }
        public int CurrentUserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            // Get current user ID for authorization
            var userId = SessionHelper.GetUserId(HttpContext.Session);
            if (!userId.HasValue)
            {
                ModelState.AddModelError("", "Unable to verify user session. Please log in again.");
                return Page();
            }

            try
            {
                // Create the pet entity from the form model
                var pet = new Pet
                {
                    OwnerId = userId.Value,
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
                    CreatedDate = DateTime.UtcNow
                };

                // Create the pet through the service
                await _petService.CreatePetAsync(pet, userId.Value);
                TempData["SuccessMessage"] = $"Pet '{Pet.Name}' has been added successfully.";
                return RedirectToPage("./Index");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the pet. Please try again.");
                // Log the actual error for debugging
                Console.WriteLine($"Error creating pet: {ex.Message}");
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            // Get current user ID
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                CurrentUserId = userId;
            }

            // Set default owner for customers
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Customer")
            {
                Pet.OwnerId = CurrentUserId;
            }
            else
            {
                // Load customer list for admin/staff
                try
                {
                    var customers = await _userService.GetUsersByRoleAsync("Customer");
                    OwnerSelectList = new SelectList(customers, "Id", "FullName");
                }
                catch (Exception)
                {
                    OwnerSelectList = new SelectList(new List<User>(), "Id", "FullName");
                }
            }
        }
    }

    public class PetCreateModel
    {
        public int OwnerId { get; set; }

        [Required(ErrorMessage = "Pet name is required")]
        [StringLength(100, ErrorMessage = "Pet name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Species is required")]
        [StringLength(50, ErrorMessage = "Species cannot exceed 50 characters")]
        public string Species { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Breed cannot exceed 100 characters")]
        public string? Breed { get; set; }

        [StringLength(20, ErrorMessage = "Gender cannot exceed 20 characters")]
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [Range(0, 1000, ErrorMessage = "Weight must be between 0 and 1000 kg")]
        [Display(Name = "Weight (kg)")]
        public decimal? Weight { get; set; }

        [StringLength(100, ErrorMessage = "Color description cannot exceed 100 characters")]
        public string? Color { get; set; }

        [StringLength(50, ErrorMessage = "Microchip ID cannot exceed 50 characters")]
        [Display(Name = "Microchip ID")]
        public string? MicrochipId { get; set; }

        [StringLength(1000, ErrorMessage = "Medical notes cannot exceed 1000 characters")]
        [Display(Name = "Medical Notes")]
        public string? MedicalNotes { get; set; }

        [Display(Name = "Active Pet")]
        public bool IsActive { get; set; } = true;
    }
}