using Microsoft.AspNetCore.SignalR;
using VetClinic.Web.Hubs;

namespace VetClinic.Web.Services
{
    public interface IAppointmentNotificationService
    {
        Task NotifyAppointmentCancelled(int appointmentId, int petOwnerId, string petName, DateTime appointmentTime);
        Task NotifyAppointmentStatusChanged(int appointmentId, string newStatus, int petOwnerId);
    }

    public class AppointmentNotificationService : IAppointmentNotificationService
    {
        private readonly IHubContext<AppointmentHub> _hubContext;
        private readonly ILogger<AppointmentNotificationService> _logger;

        public AppointmentNotificationService(
            IHubContext<AppointmentHub> hubContext,
            ILogger<AppointmentNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyAppointmentCancelled(int appointmentId, int petOwnerId, string petName, DateTime appointmentTime)
        {
            try
            {
                _logger.LogInformation($"Sending SignalR notification for cancelled appointment {appointmentId}");

                var notificationData = new
                {
                    appointmentId = appointmentId,
                    petName = petName,
                    appointmentTime = appointmentTime.ToString("MMM dd, yyyy hh:mm tt"),
                    status = "Cancelled",
                    message = $"Appointment for {petName} on {appointmentTime:MMM dd, yyyy hh:mm tt} has been cancelled."
                };

                // Notify the pet owner
                await _hubContext.Clients.Group($"User_{petOwnerId}")
                    .SendAsync("AppointmentCancelled", notificationData);

                // Notify all admin/staff users
                await _hubContext.Clients.Group("AdminStaff")
                    .SendAsync("AppointmentCancelled", notificationData);

                // Notify specific appointment watchers
                await _hubContext.Clients.Group($"Appointment_{appointmentId}")
                    .SendAsync("AppointmentCancelled", notificationData);

                _logger.LogInformation($"SignalR notification sent successfully for appointment {appointmentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SignalR notification for cancelled appointment {appointmentId}");
            }
        }

        public async Task NotifyAppointmentStatusChanged(int appointmentId, string newStatus, int petOwnerId)
        {
            try
            {
                _logger.LogInformation($"Sending SignalR notification for appointment {appointmentId} status change to {newStatus}");

                var notificationData = new
                {
                    appointmentId = appointmentId,
                    status = newStatus,
                    message = $"Appointment status has been updated to {newStatus}."
                };

                // Notify the pet owner
                await _hubContext.Clients.Group($"User_{petOwnerId}")
                    .SendAsync("AppointmentStatusChanged", notificationData);

                // Notify all admin/staff users
                await _hubContext.Clients.Group("AdminStaff")
                    .SendAsync("AppointmentStatusChanged", notificationData);

                // Notify specific appointment watchers
                await _hubContext.Clients.Group($"Appointment_{appointmentId}")
                    .SendAsync("AppointmentStatusChanged", notificationData);

                _logger.LogInformation($"SignalR notification sent successfully for appointment {appointmentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SignalR notification for appointment {appointmentId} status change");
            }
        }
    }
}
