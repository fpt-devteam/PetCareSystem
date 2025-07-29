using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VetClinic.Repository.Entities;
using VetClinic.Service.Interfaces;
using VetClinic.Web.Helpers;

namespace VetClinic.Web.Pages.Doctors
{
    public class BlockSlotsModel : PageModel
    {
        private readonly IBlockedSlotService _blockedSlotService;
        private readonly IDoctorAvailabilityService _doctorAvailabilityService;

        public BlockSlotsModel(IBlockedSlotService blockedSlotService, IDoctorAvailabilityService doctorAvailabilityService)
        {
            _blockedSlotService = blockedSlotService;
            _doctorAvailabilityService = doctorAvailabilityService;
        }

        [BindProperty]
        public BlockTimeRequest BlockRequest { get; set; } = new BlockTimeRequest();

        public IEnumerable<BlockedSlot> BlockedSlots { get; set; } = new List<BlockedSlot>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            if (userRole != "Doctor")
            {
                TempData["ErrorMessage"] = "Access denied. This page is for doctors only.";
                return RedirectToPage("/");
            }

            try
            {
                await LoadBlockedSlotsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading blocked slots. Please try again.";
                Console.WriteLine($"Error loading blocked slots: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostBlockTimeAsync()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            var userId = SessionHelper.GetUserId(HttpContext.Session);

            if (userRole != "Doctor" || !userId.HasValue)
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToPage();
            }

            if (!ModelState.IsValid)
            {
                await LoadBlockedSlotsAsync();
                return Page();
            }

            try
            {
                // Create blocked time slots based on the request
                await CreateBlockedSlotsAsync(userId.Value);
                TempData["SuccessMessage"] = "Time slot(s) blocked successfully.";
                return RedirectToPage();
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                await LoadBlockedSlotsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error blocking time slot. Please try again.";
                Console.WriteLine($"Error blocking time slot: {ex.Message}");
                await LoadBlockedSlotsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUnblockTimeAsync(int blockedSlotId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToPage("/Account/Login");
            }

            var userRole = SessionHelper.GetUserRole(HttpContext.Session);
            var userId = SessionHelper.GetUserId(HttpContext.Session);

            if (userRole != "Doctor" || !userId.HasValue)
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToPage();
            }

            try
            {
                var success = await _blockedSlotService.UnblockTimeSlotAsync(userId.Value, blockedSlotId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Time slot unblocked successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to unblock time slot or you don't have permission.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error unblocking time slot.";
                Console.WriteLine($"Error unblocking time slot: {ex.Message}");
            }

            return RedirectToPage();
        }

        private async Task LoadBlockedSlotsAsync()
        {
            var userId = SessionHelper.GetUserId(HttpContext.Session);
            if (!userId.HasValue) return;

            try
            {
                var today = DateTime.Today;
                var futureDate = today.AddDays(90); // Load next 3 months

                BlockedSlots = await _blockedSlotService.GetDoctorBlockedSlotsInRangeAsync(userId.Value, today, futureDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading blocked slots: {ex.Message}");
                BlockedSlots = new List<BlockedSlot>();
            }
        }

        private async Task CreateBlockedSlotsAsync(int doctorId)
        {
            var blockDate = BlockRequest.BlockDate;
            var timeSlots = GetTimeSlots();

            foreach (var (startTime, endTime) in timeSlots)
            {
                var startDateTime = blockDate.Add(startTime);
                var endDateTime = blockDate.Add(endTime);

                // Check if doctor is available (no conflicting appointments)
                var isAvailable = await _doctorAvailabilityService.IsDoctorAvailableAsync(doctorId, startDateTime, endDateTime);
                if (!isAvailable)
                {
                    var reason = await _doctorAvailabilityService.GetAvailabilityReasonAsync(doctorId, startDateTime, endDateTime);
                    throw new ArgumentException($"Cannot block time slot {startTime:hh\\:mm} - {endTime:hh\\:mm}: {reason}");
                }

                await _blockedSlotService.CreateBlockedSlotAsync(
                    doctorId, 
                    startDateTime, 
                    endDateTime, 
                    BlockRequest.BlockType, 
                    BlockRequest.Reason);
            }
        }

        private List<(TimeSpan start, TimeSpan end)> GetTimeSlots()
        {
            var slots = new List<(TimeSpan start, TimeSpan end)>();

            switch (BlockRequest.BlockType)
            {
                case "FullDay":
                    slots.Add((new TimeSpan(8, 0, 0), new TimeSpan(18, 0, 0)));
                    break;

                case "Morning":
                    slots.Add((new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0)));
                    break;

                case "Afternoon":
                    slots.Add((new TimeSpan(13, 0, 0), new TimeSpan(18, 0, 0)));
                    break;

                case "Custom":
                    if (BlockRequest.StartTime.HasValue && BlockRequest.EndTime.HasValue)
                    {
                        slots.Add((BlockRequest.StartTime.Value, BlockRequest.EndTime.Value));
                    }
                    break;
            }

            return slots;
        }
    }

    public class BlockTimeRequest
    {
        [Required(ErrorMessage = "Please select a date")]
        [Display(Name = "Block Date")]
        [DataType(DataType.Date)]
        public DateTime BlockDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Please select a block type")]
        [Display(Name = "Block Type")]
        public string BlockType { get; set; } = string.Empty;

        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan? EndTime { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        [Display(Name = "Reason")]
        public string? Reason { get; set; }
    }
}
