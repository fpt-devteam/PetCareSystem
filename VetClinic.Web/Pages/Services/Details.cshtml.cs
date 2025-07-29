using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Services
{
  public class DetailsModel : PageModel
  {
    private readonly IServiceService _serviceService;

    public DetailsModel(IServiceService serviceService)
    {
      _serviceService = serviceService;
    }

    public VetClinic.Repository.Entities.Service Service { get; set; } = new VetClinic.Repository.Entities.Service();
    public string UserRole { get; set; } = string.Empty;
    public bool CanEdit { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
      try
      {
        var service = await _serviceService.GetServiceByIdAsync(id);
        if (service == null)
        {
          TempData["ErrorMessage"] = "Service not found.";
          return RedirectToPage("./Index");
        }

        Service = service;

        // Check user permissions
        if (SessionHelper.IsAuthenticated(HttpContext.Session))
        {
          UserRole = SessionHelper.GetUserRole(HttpContext.Session) ?? "Customer";
          CanEdit = UserRole == "Admin";
        }
        else
        {
          UserRole = "Guest";
          CanEdit = false;
        }

        return Page();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading service details: {ex.Message}");
        TempData["ErrorMessage"] = "Error loading service details. Please try again.";
        return RedirectToPage("./Index");
      }
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(int serviceId, bool isActive)
    {
      if (!SessionHelper.IsAuthenticated(HttpContext.Session))
      {
        return RedirectToPage("/Account/Login");
      }

      var userRole = SessionHelper.GetUserRole(HttpContext.Session);
      if (userRole != "Admin")
      {
        TempData["ErrorMessage"] = "You are not authorized to modify services. Admin access required.";
        return RedirectToPage();
      }

      try
      {
        var userId = SessionHelper.GetUserId(HttpContext.Session);
        if (!userId.HasValue)
        {
          TempData["ErrorMessage"] = "Unable to verify user session.";
          return RedirectToPage();
        }

        var service = await _serviceService.GetServiceByIdAsync(serviceId);
        if (service == null)
        {
          TempData["ErrorMessage"] = "Service not found.";
          return RedirectToPage("./Index");
        }

        service.IsActive = isActive;
        await _serviceService.UpdateServiceAsync(service, userId.Value);

        var action = isActive ? "activated" : "deactivated";
        TempData["SuccessMessage"] = $"Service has been {action} successfully.";
      }
      catch (Exception ex)
      {
        TempData["ErrorMessage"] = "Error updating service status.";
        Console.WriteLine($"Error toggling service status: {ex.Message}");
      }

      return RedirectToPage(new { id = serviceId });
    }
  }
}