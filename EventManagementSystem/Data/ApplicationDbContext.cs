using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Models.Entities;

namespace EventManagementSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<EventSchedule> EventSchedules { get; set; }
    public DbSet<Certificate> Certificates { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(a => a.Id)
                    .HasColumnName("UserId")
                    .HasDefaultValueSql("NEWID()");
            entity.Property(e => e.Email).HasColumnName("Email").IsRequired().HasMaxLength(256);
            entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(256);
            entity.Property(e => e.Phone).HasColumnName("Phone").HasMaxLength(20);
            entity.Property(e => e.Address).HasColumnName("Address").HasMaxLength(500);
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasMany(e => e.Certificates)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.EventsAsSpeaker)
                .WithOne(es => es.Speaker)
                .HasForeignKey(es => es.SpeakerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<EventSchedule>(entity =>
        {
            entity.ToTable("EventSchedules");
            entity.HasKey(e => e.EventScheduleId);
            entity.Property(a => a.EventScheduleId)
                    .HasColumnName("EventScheduleId")
                    .HasDefaultValueSql("NEWID()");
            entity.Property(e => e.Title).HasColumnName("Title").IsRequired().HasMaxLength(256);
            entity.Property(e => e.Description).HasColumnName("Description").IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Location).HasColumnName("Location").IsRequired().HasMaxLength(500);
            
            entity.HasMany(e => e.Certificates)
                .WithOne(c => c.EventSchedule)
                .HasForeignKey(c => c.EventScheduleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Certificate>(entity =>
        {
            entity.ToTable("Certificates");
            entity.HasKey(e => e.CertificateId);
            entity.Property(a => a.CertificateId)
                    .HasColumnName("CertificateId")
                    .HasDefaultValueSql("NEWID()");
            entity.Property(e => e.CertificateNumber).HasColumnName("CertificateNumber").IsRequired().HasMaxLength(100);
            entity.Property(e => e.PdfPath).HasColumnName("PdfPath").IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.CertificateNumber).IsUnique();
        });


        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        builder.Entity<User>().HasData(
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Name = "Admin User",
                UserType = UserType.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "speaker@example.com",
                Name = "John Speaker",
                UserType = UserType.Speaker,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
