using VetClinic.Repository;
using VetClinic.Service;
using VetClinic.Repository.Data;
using VetClinic.Web.Hubs;
using VetClinic.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add SignalR
builder.Services.AddSignalR();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add 3-layer architecture services
builder.Services.AddRepository(builder.Configuration);
builder.Services.AddServices();

// Add SignalR notification service
builder.Services.AddScoped<IAppointmentNotificationService, AppointmentNotificationService>();

var app = builder.Build();

// Seed the database in development environment
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<VetClinicDbContext>();
        await DbSeeder.SeedAsync(context);
    }
}
        
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapRazorPages();
app.MapHub<AppointmentHub>("/appointmentHub");

app.Run();
