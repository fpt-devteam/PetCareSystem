using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Services
{
  public class EditModel : PageModel
  {
    private readonly IServiceService _serviceService;

    public EditModel(IServiceService serviceService)
    {
      _serviceService = serviceService;
    }

    [BindProperty]
    public ServiceEditModel Service { get; set; } = new ServiceEditModel();

    public async Task<IActionResult> OnGetAsync(int id)
    {
      if (!SessionHelper.IsAuthenticated(HttpContext.Session))
      {
        return RedirectToPage("/Account/Login");
      }

      // Only admins can edit services
      if (!SessionHelper.IsInRole(HttpContext.Session, "Admin"))
      {
        return RedirectToPage("/Index");
      }

      try
      {
        var service = await _serviceService.GetServiceByIdAsync(id);
        if (service == null)
        {
          TempData["ErrorMessage"] = "Service not found.";
          return RedirectToPage("./Index");
        }

        // Map entity to edit model
        Service = new ServiceEditModel
        {
          Id = service.Id,
          Name = service.Name,
          Description = service.Description,
          DurationMinutes = service.DurationMinutes,
          Price = service.Price,
          IsActive = service.IsActive
        };

        return Page();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading service for edit: {ex.Message}");
        TempData["ErrorMessage"] = "Error loading service. Please try again.";
        return RedirectToPage("./Index");
      }
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!SessionHelper.IsAuthenticated(HttpContext.Session))
      {
        return RedirectToPage("/Account/Login");
      }

      // Only admins can edit services
      if (!SessionHelper.IsInRole(HttpContext.Session, "Admin"))
      {
        return RedirectToPage("/Index");
      }

      if (!ModelState.IsValid)
      {
        return Page();
      }

      try
      {
        var userId = SessionHelper.GetUserId(HttpContext.Session);
        if (!userId.HasValue)
        {
          TempData["ErrorMessage"] = "Unable to verify user session.";
          return Page();
        }

        // Check if service exists
        var existingService = await _serviceService.GetServiceByIdAsync(Service.Id);
        if (existingService == null)
        {
          TempData["ErrorMessage"] = "Service not found.";
          return RedirectToPage("./Index");
        }

        // Check if name is unique (excluding current service)
        if (!await _serviceService.IsServiceNameUniqueAsync(Service.Name, Service.Id))
        {
          ModelState.AddModelError(nameof(Service.Name), "A service with this name already exists.");
          return Page();
        }

        // Update the service entity
        existingService.Name = Service.Name;
        existingService.Description = Service.Description;
        existingService.DurationMinutes = Service.DurationMinutes;
        existingService.Price = Service.Price;
        existingService.IsActive = Service.IsActive;

        await _serviceService.UpdateServiceAsync(existingService, userId.Value);
        TempData["SuccessMessage"] = $"Service '{Service.Name}' has been updated successfully.";
        return RedirectToPage("./Index");
      }
      catch (ArgumentException ex)
      {
        ModelState.AddModelError("", ex.Message);
        return Page();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error updating service: {ex.Message}");
        ModelState.AddModelError("", "An error occurred while updating the service. Please try again.");
        return Page();
      }
    }
  }

  public class ServiceEditModel
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "Service name is required")]
    [StringLength(255, ErrorMessage = "Service name cannot exceed 255 characters")]
    [Display(Name = "Service Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Duration is required")]
    [Range(15, 480, ErrorMessage = "Duration must be between 15 minutes and 8 hours")]
    [Display(Name = "Duration (Minutes)")]
    public int DurationMinutes { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 9999.99, ErrorMessage = "Price must be between $0.01 and $9999.99")]
    [Display(Name = "Price ($)")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
  }
}