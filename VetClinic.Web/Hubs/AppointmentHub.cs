using Microsoft.AspNetCore.SignalR;

namespace VetClinic.Web.Hubs
{
    public class AppointmentHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task JoinAppointmentGroup(string appointmentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Appointment_{appointmentId}");
        }

        public async Task LeaveAppointmentGroup(string appointmentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Appointment_{appointmentId}");
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"SignalR Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"SignalR Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
