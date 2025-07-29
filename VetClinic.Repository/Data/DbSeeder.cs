using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Entities;
using System.Security.Cryptography;
using System.Text;

namespace VetClinic.Repository.Data
{
    public static class DbSeeder
    {
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static async Task DeleteAllData(VetClinicDbContext context)
        {
            context.Reminders.RemoveRange(context.Reminders);
            context.Feedback.RemoveRange(context.Feedback);

            context.MedicalRecords.RemoveRange(context.MedicalRecords);
            context.Invoices.RemoveRange(context.Invoices);

            context.Vaccinations.RemoveRange(context.Vaccinations);

            context.Appointments.RemoveRange(context.Appointments);

            context.Pets.RemoveRange(context.Pets);

            context.Services.RemoveRange(context.Services);

            context.Users.RemoveRange(context.Users);

            await context.SaveChangesAsync();
        }

        public static async Task SeedAsync(VetClinicDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            await DeleteAllData(context);

            // Check if data already exists
            // if (context.Users.Any())
            // {
            //     return;
            // }

            // Seed Users
            var users = new List<User>
            {
                // Admin User
                new User
                {
                    FullName = "System Administrator",
                    Email = "admin@vetclinic.com",
                    PasswordHash = HashPassword("Admin123!"),
                    Role = "Admin",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-30)
                },
                // Manager
                new User
                {
                    FullName = "Dr. Sarah Johnson",
                    Email = "manager@vetclinic.com",
                    PasswordHash = HashPassword("Manager123!"),
                    Role = "Manager",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-25)
                },
                // Doctors
                new User
                {
                    FullName = "Dr. Michael Chen",
                    Email = "dr.chen@vetclinic.com",
                    PasswordHash = HashPassword("Doctor123!"),
                    Role = "Doctor",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-20)
                },
                new User
                {
                    FullName = "Dr. Emily Rodriguez",
                    Email = "dr.rodriguez@vetclinic.com",
                    PasswordHash = HashPassword("Doctor123!"),
                    Role = "Doctor",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-18)
                },
                // Staff
                new User
                {
                    FullName = "Lisa Thompson",
                    Email = "staff@vetclinic.com",
                    PasswordHash = HashPassword("Staff123!"),
                    Role = "Staff",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-15)
                },
                // Customers
                new User
                {
                    FullName = "John Smith",
                    Email = "john.smith@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    Role = "Customer",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-10)
                },
                new User
                {
                    FullName = "Maria Garcia",
                    Email = "maria.garcia@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    Role = "Customer",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-8)
                },
                new User
                {
                    FullName = "David Wilson",
                    Email = "david.wilson@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    Role = "Customer",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-5)
                },
                new User
                {
                    FullName = "Jennifer Brown",
                    Email = "jennifer.brown@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    Role = "Customer",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-3)
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Get customer IDs for pet ownership
            var customers = context.Users.Where(u => u.Role == "Customer").ToList();
            var doctors = context.Users.Where(u => u.Role == "Doctor").ToList();

            // Seed Services
            var services = new List<Service>
            {
                new Service
                {
                    Name = "General Checkup",
                    Description = "Comprehensive health examination for pets",
                    Price = 75.00m,
                    DurationMinutes = 30,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-20)
                },
                new Service
                {
                    Name = "Vaccination",
                    Description = "Standard vaccination package",
                    Price = 50.00m,
                    DurationMinutes = 15,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-20)
                },
                new Service
                {
                    Name = "Dental Cleaning",
                    Description = "Professional dental cleaning and examination",
                    Price = 150.00m,
                    DurationMinutes = 60,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-20)
                },
                new Service
                {
                    Name = "Surgery Consultation",
                    Description = "Pre-surgery examination and consultation",
                    Price = 125.00m,
                    DurationMinutes = 45,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-20)
                },
                new Service
                {
                    Name = "Emergency Care",
                    Description = "Emergency treatment and stabilization",
                    Price = 200.00m,
                    DurationMinutes = 90,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-20)
                },
                new Service
                {
                    Name = "Grooming",
                    Description = "Full grooming service including bath and nail trim",
                    Price = 45.00m,
                    DurationMinutes = 60,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-20)
                }
            };

            context.Services.AddRange(services);
            await context.SaveChangesAsync();

            // Seed Pets
            var pets = new List<Pet>
            {
                new Pet
                {
                    OwnerId = customers[0].Id,
                    Name = "Buddy",
                    Species = "Dog",
                    Breed = "Golden Retriever",
                    BirthDate = DateTime.Now.AddYears(-3),
                    Weight = 28.5m,
                    CreatedDate = DateTime.Now.AddDays(-10)
                },
                new Pet
                {
                    OwnerId = customers[0].Id,
                    Name = "Whiskers",
                    Species = "Cat",
                    Breed = "Persian",
                    BirthDate = DateTime.Now.AddYears(-2),
                    Weight = 4.2m,
                    CreatedDate = DateTime.Now.AddDays(-8)
                },
                new Pet
                {
                    OwnerId = customers[1].Id,
                    Name = "Max",
                    Species = "Dog",
                    Breed = "German Shepherd",
                    BirthDate = DateTime.Now.AddYears(-5),
                    Weight = 35.0m,
                    CreatedDate = DateTime.Now.AddDays(-7)
                },
                new Pet
                {
                    OwnerId = customers[1].Id,
                    Name = "Luna",
                    Species = "Cat",
                    Breed = "Siamese",
                    BirthDate = DateTime.Now.AddYears(-1),
                    Weight = 3.8m,
                    CreatedDate = DateTime.Now.AddDays(-6)
                },
                new Pet
                {
                    OwnerId = customers[2].Id,
                    Name = "Charlie",
                    Species = "Dog",
                    Breed = "Labrador",
                    BirthDate = DateTime.Now.AddYears(-4),
                    Weight = 32.1m,
                    CreatedDate = DateTime.Now.AddDays(-5)
                },
                new Pet
                {
                    OwnerId = customers[3].Id,
                    Name = "Bella",
                    Species = "Dog",
                    Breed = "French Bulldog",
                    BirthDate = DateTime.Now.AddYears(-2),
                    Weight = 12.5m,
                    CreatedDate = DateTime.Now.AddDays(-3)
                },
                new Pet
                {
                    OwnerId = customers[3].Id,
                    Name = "Oliver",
                    Species = "Cat",
                    Breed = "Maine Coon",
                    BirthDate = DateTime.Now.AddYears(-3),
                    Weight = 6.8m,
                    CreatedDate = DateTime.Now.AddDays(-2)
                }
            };

            context.Pets.AddRange(pets);
            await context.SaveChangesAsync();

            // Seed Appointments
            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    PetId = pets[0].Id,
                    DoctorId = doctors[0].Id,
                    ServiceId = services[0].Id,
                    AppointmentTime = DateTime.Now.AddDays(1).AddHours(10),
                    Status = "Scheduled",
                    Notes = "Annual checkup",
                    CreatedDate = DateTime.Now.AddDays(-2)
                },
                new Appointment
                {
                    PetId = pets[1].Id,
                    DoctorId = doctors[1].Id,
                    ServiceId = services[1].Id,
                    AppointmentTime = DateTime.Now.AddDays(2).AddHours(14),
                    Status = "Scheduled",
                    Notes = "Rabies vaccination due",
                    CreatedDate = DateTime.Now.AddDays(-1)
                },
                new Appointment
                {
                    PetId = pets[2].Id,
                    DoctorId = doctors[0].Id,
                    ServiceId = services[2].Id,
                    AppointmentTime = DateTime.Now.AddDays(3).AddHours(9),
                    Status = "Scheduled",
                    Notes = "Dental cleaning appointment",
                    CreatedDate = DateTime.Now.AddDays(-1)
                },
                // Past appointments
                new Appointment
                {
                    PetId = pets[0].Id,
                    DoctorId = doctors[1].Id,
                    ServiceId = services[0].Id,
                    AppointmentTime = DateTime.Now.AddDays(-5).AddHours(11),
                    Status = "Completed",
                    Notes = "Regular checkup completed",
                    CreatedDate = DateTime.Now.AddDays(-6)
                },
                new Appointment
                {
                    PetId = pets[3].Id,
                    DoctorId = doctors[0].Id,
                    ServiceId = services[1].Id,
                    AppointmentTime = DateTime.Now.AddDays(-3).AddHours(15),
                    Status = "Completed",
                    Notes = "Vaccination completed successfully",
                    CreatedDate = DateTime.Now.AddDays(-4)
                }
            };

            context.Appointments.AddRange(appointments);
            await context.SaveChangesAsync();

            // Seed Vaccinations
            var vaccinations = new List<Vaccination>
            {
                new Vaccination
                {
                    PetId = pets[0].Id,
                    VaccineName = "Rabies",
                    DueDate = DateTime.Now.AddMonths(6),
                    Status = "Due",
                    CreatedDate = DateTime.Now.AddDays(-10)
                },
                new Vaccination
                {
                    PetId = pets[1].Id,
                    VaccineName = "FVRCP",
                    DueDate = DateTime.Now.AddDays(30),
                    Status = "Due",
                    CreatedDate = DateTime.Now.AddDays(-8)
                },
                new Vaccination
                {
                    PetId = pets[2].Id,
                    VaccineName = "Rabies",
                    DueDate = DateTime.Now.AddDays(-5),
                    Status = "Overdue",
                    CreatedDate = DateTime.Now.AddDays(-365)
                },
                new Vaccination
                {
                    PetId = pets[3].Id,
                    VaccineName = "DHPP",
                    DueDate = DateTime.Now.AddDays(-10),
                    CompletedDate = DateTime.Now.AddDays(-10),
                    Status = "Completed",
                    CreatedDate = DateTime.Now.AddDays(-15)
                }
            };

            context.Vaccinations.AddRange(vaccinations);
            await context.SaveChangesAsync();

            // Seed Medical Records for completed appointments
            var completedAppointments = appointments.Where(a => a.Status == "Completed").ToList();
            var medicalRecords = new List<MedicalRecord>();

            foreach (var appointment in completedAppointments)
            {
                medicalRecords.Add(new MedicalRecord
                {
                    AppointmentId = appointment.Id,
                    PetId = appointment.PetId,
                    DoctorId = appointment.DoctorId,
                    VisitDate = appointment.AppointmentTime,
                    Diagnosis = "Healthy - routine checkup completed",
                    TreatmentNotes = "No issues found during examination. Pet is in good health.",
                    Prescription = appointment.ServiceId == services[1].Id ? "Rabies vaccine administered" : null,
                    CreatedDate = appointment.AppointmentTime.AddHours(1)
                });
            }

            context.MedicalRecords.AddRange(medicalRecords);
            await context.SaveChangesAsync();

            // Seed Invoices for completed appointments
            var invoices = new List<Invoice>();
            foreach (var appointment in completedAppointments)
            {
                var service = services.First(s => s.Id == appointment.ServiceId);
                invoices.Add(new Invoice
                {
                    AppointmentId = appointment.Id,
                    TotalAmount = service.Price,
                    Status = "Paid",
                    PaidDate = appointment.AppointmentTime.AddDays(1),
                    CreatedDate = appointment.AppointmentTime.AddHours(2)
                });
            }

            context.Invoices.AddRange(invoices);
            await context.SaveChangesAsync();

            // Seed Feedback for completed appointments
            var feedback = new List<Feedback>
            {
                new Feedback
                {
                    AppointmentId = completedAppointments[0].Id,
                    CustomerId = pets.First(p => p.Id == completedAppointments[0].PetId).OwnerId,
                    Rating = 5,
                    Comment = "Excellent service! Dr. Chen was very thorough and caring with Buddy.",
                    Approved = true,
                    CreatedDate = completedAppointments[0].AppointmentTime.AddDays(1)
                },
                new Feedback
                {
                    AppointmentId = completedAppointments[1].Id,
                    CustomerId = pets.First(p => p.Id == completedAppointments[1].PetId).OwnerId,
                    Rating = 4,
                    Comment = "Great experience. Very professional staff and clean facility.",
                    Approved = true,
                    CreatedDate = completedAppointments[1].AppointmentTime.AddDays(2)
                }
            };

            context.Feedback.AddRange(feedback);
            await context.SaveChangesAsync();

            // Seed Reminders
            var reminders = new List<Reminder>
            {
                new Reminder
                {
                    UserId = customers[0].Id,
                    Type = "Appointment",
                    Title = "Upcoming Appointment Reminder",
                    Message = "Your pet Buddy has an appointment tomorrow at 10:00 AM",
                    SendAt = DateTime.Now.AddHours(12),
                    Status = "Pending",
                    CreatedDate = DateTime.Now
                },
                new Reminder
                {
                    UserId = customers[1].Id,
                    Type = "Vaccination",
                    Title = "Vaccination Due",
                    Message = "Luna's FVRCP vaccination is due in 30 days",
                    SendAt = DateTime.Now.AddDays(25),
                    Status = "Pending",
                    CreatedDate = DateTime.Now
                }
            };

            context.Reminders.AddRange(reminders);
            await context.SaveChangesAsync();
        }
    }
}