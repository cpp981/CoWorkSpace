using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Models;
using System;

namespace CoWorkSpace.Api.Data
{
    public class CoWorkSpaceContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Space> Spaces { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Log> Logs { get; set; }

        public CoWorkSpaceContext(DbContextOptions<CoWorkSpaceContext> options)
     : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Índices
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Log>().HasIndex(l => l.Timestamp);
            modelBuilder.Entity<Log>().HasIndex(l => l.EventType);

            // Relación recursiva User -> Provider
            modelBuilder.Entity<User>()
                .HasOne(u => u.Provider)
                .WithMany(p => p.Admins)
                .HasForeignKey(u => u.ProviderId)
                .HasConstraintName("FK_Users_Users_ProviderId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Relación User -> Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .IsRequired();

            // Relación Space -> Admin
            modelBuilder.Entity<Space>()
                .HasOne(s => s.Admin)
                .WithMany()
                .HasForeignKey(s => s.AdminId)
                .HasConstraintName("FK_Spaces_Users_AdminId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Índice para ProviderId
            modelBuilder.Entity<Space>()
                .HasIndex(s => s.ProviderId);

            // Relación Space -> Provider
            modelBuilder.Entity<Space>()
                .HasOne(s => s.Provider)
                .WithMany()
                .HasForeignKey(s => s.ProviderId)
                .HasConstraintName("FK_Spaces_Users_ProviderId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Filtro global para Spaces
            modelBuilder.Entity<Space>()
                .HasQueryFilter(s => !s.IsDeleted && s.Admin != null && s.Admin.RoleId == 2 && s.Admin.ProviderId == s.ProviderId);

            // Relaciones de Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .HasConstraintName("FK_Bookings_Users_UserId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Space)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.SpaceId)
                .HasConstraintName("FK_Bookings_Spaces_SpaceId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Relaciones de Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .HasConstraintName("FK_Reviews_Users_UserId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Space)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.SpaceId)
                .HasConstraintName("FK_Reviews_Spaces_SpaceId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Review>()
                .HasQueryFilter(r => r.User != null && r.User.RoleId == 4);

            // Relaciones de Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .HasConstraintName("FK_Payments_Users_UserId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .HasConstraintName("FK_Payments_Bookings_BookingId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Space)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SpaceId)
                .HasConstraintName("FK_Payments_Spaces_SpaceId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .HasQueryFilter(p => p.User != null &&
                    (p.User.RoleId == 4 || p.User.RoleId == 2 || p.User.RoleId == 1));

            // Nueva entidad: SpaceMembership
            modelBuilder.Entity<SpaceMembership>()
                .HasKey(sm => sm.Id);

            modelBuilder.Entity<SpaceMembership>()
                .HasOne(sm => sm.User)
                .WithMany()
                .HasForeignKey(sm => sm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SpaceMembership>()
                .HasOne(sm => sm.Space)
                .WithMany()
                .HasForeignKey(sm => sm.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relaciones de Log
            modelBuilder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .HasConstraintName("FK_Logs_Users_UserId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "SuperAdmin" },
                new Role { RoleId = 2, RoleName = "Admin" },
                new Role { RoleId = 3, RoleName = "Provider" },
                new Role { RoleId = 4, RoleName = "Client" }
            );

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "superadmin@coworkspace.com",
                    PasswordHash = "$2a$11$LSiUCLVeVeOynzKQVsM2XOq4jS3IBhsJ.VzouEqCmjQjhty4l.3Pa",
                    Name = "Super Admin",
                    RoleId = 1,
                    ProviderId = null
                },
                new User
                {
                    Id = 2,
                    Email = "provider@coworkspace.com",
                    PasswordHash = "$2a$11$ix99XlIasCCcYr/Zz5AwzO5TTr.Zv.ykHWwRULTo4NyWTSr9J3W5W",
                    Name = "Test Provider",
                    RoleId = 3,
                    ProviderId = null
                },
                new User
                {
                    Id = 3,
                    Email = "admin@coworkspace.com",
                    PasswordHash = "$2a$11$Kn0nDdok.EqeppL6E0rTje40JdNq0qP8Pz.D/YtISJBgH1UgRrvqG",
                    Name = "Test Admin",
                    RoleId = 2,
                    ProviderId = 2
                },
                new User
                {
                    Id = 4,
                    Email = "client@coworkspace.com",
                    PasswordHash = "$2a$11$R6e5nDM1HoXKHFhxALf4B.jQpJ7tko/p5zY.R.e7QCloUrOEMtoRe",
                    Name = "Test Client",
                    RoleId = 4,
                    ProviderId = null
                }
            );

            // Seed Space
            modelBuilder.Entity<Space>().HasData(
                new Space
                {
                    Id = 1,
                    Name = "Test Space",
                    AdminId = 3,
                    ProviderId = 2,
                    IsPublic = true,
                    Price = 50.00m,
                    City = "Madrid",
                    IsDeleted = false
                }
            );

            // Seed Booking
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = 1,
                    UserId = 4,
                    SpaceId = 1,
                    StartTime = new DateTime(2025, 5, 21, 10, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2025, 5, 21, 12, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed Review
            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    Id = 1,
                    UserId = 4,
                    SpaceId = 1,
                    Rating = 5,
                    Comment = "Excelente espacio de trabajo!"
                }
            );

            // Seed Payments
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    Id = 1,
                    BookingId = 1,
                    SpaceId = null,
                    UserId = 4,
                    Amount = 100.00m,
                    Status = "CREADO"
                },
                new Payment
                {
                    Id = 2,
                    BookingId = null,
                    SpaceId = 1,
                    UserId = 3,
                    Amount = 50.00m,
                    Status = "CREADO"
                }
            );

            // Seed Log
            modelBuilder.Entity<Log>().HasData(
                new Log
                {
                    Id = 1,
                    Timestamp = new DateTime(2025, 5, 19, 23, 59, 59, DateTimeKind.Utc),
                    EventType = "SystemStartup",
                    UserId = null,
                    Details = "Sistema iniciado correctamente."
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
