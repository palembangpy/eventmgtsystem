using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Core.Models.Entities;

namespace EventManagementSystem.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<EventSchedule> EventSchedules { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }
    public DbSet<Tokenizer> Tokenizers {get; set;}


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

            entity.HasMany(e => e.EventParticipants)
                .WithOne(ep => ep.User)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Restrict);
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
            entity.Property(e => e.Tags).HasColumnName("Tags").HasMaxLength(500);
            
            entity.HasMany(e => e.Certificates)
                .WithOne(c => c.EventSchedule)
                .HasForeignKey(c => c.EventScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Participants)
                .WithOne(ep => ep.EventSchedule)
                .HasForeignKey(ep => ep.EventScheduleId)
                .OnDelete(DeleteBehavior.Cascade);
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

        builder.Entity<EventParticipant>(entity =>
        {
            entity.ToTable("EventParticipants");
            entity.Property(e => e.EventParticipantId)
                .HasColumnName("EventParticipantId")
                .HasDefaultValueSql("NEWID()");
            entity.HasKey(e => e.EventParticipantId);
            entity.HasIndex(e => new { e.EventScheduleId, e.UserId }).IsUnique();
        });

        builder.Entity<Tokenizer>(entity =>
        {
            entity.ToTable("Tokenizers");
            entity.HasKey(e => e.TokenId);
            entity.Property(e => e.TokenId).HasColumnName("TokenId").HasDefaultValueSql("NEWID()");
            entity.Property(e => e.TokenName).HasColumnName("TokenName").IsRequired().HasMaxLength(100);
            entity.Property(e => e.TokenHash).HasColumnName("TokenHash").IsRequired().HasMaxLength(500);
            entity.Property(e => e.Salt).HasColumnName("Salt").IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.TokenHash).IsUnique();
        });

        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        builder.Entity<User>().HasData(
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@eventms.com",
                Name = "System Administrator",
                UserType = UserType.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "john.speaker@eventms.com",
                Name = "John Doe",
                UserType = UserType.Speaker,
                Phone = "+1234567890",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "jane.volunteer@eventms.com",
                Name = "Jane Smith",
                UserType = UserType.Volunteer,
                Phone = "+0987654321",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
