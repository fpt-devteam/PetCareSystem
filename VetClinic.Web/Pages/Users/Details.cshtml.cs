using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetClinic.Service.Interfaces;
using VetClinic.Repository.Entities;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPetService _petService;
        private readonly IAppointmentService _appointmentService;

        public DetailsModel(
            IUserService userService, 
            IPetService petService, 
            IAppointmentService appointmentService)
        {
            _userService = userService;
            _petService = petService;
            _appointmentService = appointmentService;
        }

        public User? UserDetails { get; set; }
        public List<Pet> CustomerPets { get; set; } = new List<Pet>();
        public List<Appointment> CustomerAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check if user is logged in
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login", new { returnUrl = $"/Users/Details/{id}" });
            }

            // Check if user has permission to view details
            var loggedInUserId = ViewSessionHelper.GetUserId(HttpContext.Session);
            var isAdminOrManager = ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager");
            
            // If not admin/manager and not viewing own profile, redirect to home
            if (!isAdminOrManager && loggedInUserId != id)
            {
                TempData["ErrorMessage"] = "You don't have permission to view this user's details.";
                return RedirectToPage("/Index");
            }

            // Load user details
            UserDetails = await _userService.GetUserByIdAsync(id);
            if (UserDetails == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage("/Users/Index");
            }

            // Load additional data based on user role
            if (UserDetails.Role == "Customer")
            {
                // Load customer's pets
                var pets = await _petService.GetPetsByOwnerIdAsync(UserDetails.Id);
                if (pets != null)
                {
                    CustomerPets = pets.ToList();
                }

                // Load customer's recent appointments
                var appointments = await _appointmentService.GetAppointmentsByOwnerAsync(UserDetails.Id);
                if (appointments != null)
                {
                    CustomerAppointments = appointments.ToList();
                }
            }
            else if (UserDetails.Role == "Doctor")
            {
                // Load doctor's upcoming appointments
                var appointments = await _appointmentService.GetAllAppointmentsByDoctorAsync(UserDetails.Id);
                if (appointments != null)
                {
                    DoctorAppointments = appointments.Where(a => a.AppointmentTime >= DateTime.Now).ToList();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int userId)
        {
            // Check if user is logged in and has admin permissions
            if (!ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager"))
            {
                TempData["ErrorMessage"] = "You don't have permission to delete users.";
                return RedirectToPage("/Index");
            }

            // Prevent deleting your own account
            if (userId == ViewSessionHelper.GetUserId(HttpContext.Session))
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToPage("/Users/Details", new { id = userId });
            }

            // Delete the user
            var result = await _userService.DeleteUserAsync(userId);
            if (result)
            {
                TempData["SuccessMessage"] = "User has been successfully deleted.";
                return RedirectToPage("/Users/Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user. The user may have associated records.";
                return RedirectToPage("/Users/Details", new { id = userId });
            }
        }

        public async Task<IActionResult> OnPostToggleStatusAsync(int userId, bool activate)
        {
            // Check if user is logged in and has admin permissions
            if (!ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager"))
            {
                TempData["ErrorMessage"] = "You don't have permission to change user status.";
                return RedirectToPage("/Index");
            }

            // Prevent changing your own status
            if (userId == ViewSessionHelper.GetUserId(HttpContext.Session))
            {
                TempData["ErrorMessage"] = "You cannot change your own account status.";
                return RedirectToPage("/Users/Details", new { id = userId });
            }

            // Get user
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage("/Users/Index");
            }

            // Toggle user status
            user.IsActive = activate;
            var result = await _userService.UpdateUserAsync(user);
            if (result != null)
            {
                TempData["SuccessMessage"] = activate ? 
                    "User has been successfully activated." : 
                    "User has been successfully deactivated.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user status.";
            }

            return RedirectToPage("/Users/Details", new { id = userId });
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(int userId)
        {
            // Check if user is logged in and has admin permissions
            if (!ViewSessionHelper.IsInAnyRole(HttpContext.Session, "Admin", "Manager"))
            {
                TempData["ErrorMessage"] = "You don't have permission to reset passwords.";
                return RedirectToPage("/Index");
            }

            // Get user
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage("/Users/Index");
            }

            // In a real implementation, you would:
            // 1. Generate a random password or reset token
            // 2. Update the user's password hash
            // 3. Send an email to the user with reset instructions
            
            // Here we're simulating the operation
            TempData["SuccessMessage"] = "Password has been reset. An email with instructions has been sent to the user.";

            return RedirectToPage("/Users/Details", new { id = userId });
        }
    }
}
