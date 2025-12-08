using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CalendarApp.Domain.Entities;
using CalendarApp.Infrastructure.Database.Seeding;

namespace CalendarApp.Infrastructure.Database
{
    public class CalendarDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventAttendee> EventAttendees { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public CalendarDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User table
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.FullName).HasMaxLength(100);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(u => u.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
                entity.HasIndex(u => u.UserName).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Configure Event table
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Events");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Location).HasMaxLength(255);
                entity.Property(e => e.Color).HasMaxLength(7);
                entity.Property(e => e.Visibility).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
                entity.HasOne(e => e.Owner).WithMany(u => u.OwnedEvents).HasForeignKey(e => e.OwnerId);
            });

            // Configure EventAttendee table
            modelBuilder.Entity<EventAttendee>(entity =>
            {
                entity.ToTable("EventAttendees");
                entity.HasKey(ea => new { ea.EventId, ea.UserId });
                entity.Property(ea => ea.Status).HasConversion<string>();
                entity.Property(ea => ea.Role).HasConversion<string>();
                entity.Property(ea => ea.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(ea => ea.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
                entity.HasOne(ea => ea.Event).WithMany(e => e.EventAttendees).HasForeignKey(ea => ea.EventId);
                entity.HasOne(ea => ea.User).WithMany(u => u.EventAttendees).HasForeignKey(ea => ea.UserId);
            });

            // Configure Notification table
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();
                entity.Property(n => n.Type).HasConversion<string>();
                entity.Property(n => n.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(n => n.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
                entity.HasOne(n => n.User).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId);
                entity.HasOne(n => n.Event).WithMany(e => e.Notifications).HasForeignKey(n => n.EventId);
            });

            // Configure EventAttendee composite key
            modelBuilder.Entity<EventAttendee>()
                .HasKey(ea => new { ea.EventId, ea.UserId });

            // Configure relationships
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Owner)
                .WithMany(u => u.OwnedEvents)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventAttendee>()
                .HasOne(ea => ea.Event)
                .WithMany(e => e.EventAttendees)
                .HasForeignKey(ea => ea.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventAttendee>()
                .HasOne(ea => ea.User)
                .WithMany(u => u.EventAttendees)
                .HasForeignKey(ea => ea.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Event)
                .WithMany(e => e.Notifications)
                .HasForeignKey(n => n.EventId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed data (excluding users - they are created by RoleSeeder with proper password hashes)
            // Events, EventAttendees, and Notifications will be created by RoleSeeder after users exist
            var calendarInit = new CalendarDataInit();
            // modelBuilder.Entity<Event>().HasData(calendarInit.GenerateEvents());
            // modelBuilder.Entity<EventAttendee>().HasData(calendarInit.GenerateEventAttendees());
            // modelBuilder.Entity<Notification>().HasData(calendarInit.GenerateNotifications());
        }
    }
}
