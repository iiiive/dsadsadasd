using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApungLourdesWebApi.Models;

public partial class ApunglourdesDbContext : DbContext
{
    public ApunglourdesDbContext()
    {
    }

    public ApunglourdesDbContext(DbContextOptions<ApunglourdesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amanu> Amanus { get; set; }
    public virtual DbSet<Documentrequest> Documentrequests { get; set; }
    public virtual DbSet<Donation> Donations { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Serviceschedule> Serviceschedules { get; set; }
    public virtual DbSet<Serviceschedulerequirement> Serviceschedulerequirements { get; set; }
    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<User> Users { get; set; }

    // ✅ REMOVE OnConfiguring
    // You already set the connection string in Program.cs via AddDbContext()
    // So keeping OnConfiguring will hardcode credentials and triggers the scaffolding warning.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Amanu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("amanu");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.Reading).HasMaxLength(255);
            entity.Property(e => e.Scripture).HasMaxLength(255);
            entity.Property(e => e.Theme).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Type).HasColumnType("enum('homily','gospel')");
        });

        modelBuilder.Entity<Documentrequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("documentrequests");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BridesFullName).HasMaxLength(100);
            entity.Property(e => e.ChildName).HasMaxLength(150);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.EmailAddress).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.FullNameDeceased).HasMaxLength(100);
            entity.Property(e => e.GroomsFullName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("timestamp");
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.NumberOfCopies)
                .HasDefaultValueSql("'1 Copy'")
                .HasColumnType("enum('1 Copy','2 Copies','3 Copies','4 Copies','5 Copies')");
            entity.Property(e => e.PurposeOfRequest).HasColumnType("text");
            entity.Property(e => e.RelationRequestor).HasMaxLength(100);
        });

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.DonationId).HasName("PRIMARY");

            entity.ToTable("donations");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.DonationId).HasColumnType("int(11)");
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreatedBy).HasColumnType("int(11)");
            entity.Property(e => e.DonationType).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.ModifiedBy).HasColumnType("int(11)");
            entity.Property(e => e.ReferenceNo).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasColumnType("text");
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.User).WithMany(p => p.Donations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("donations_ibfk_1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreatedBy).HasColumnType("int(11)");
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.ModifiedBy).HasColumnType("int(11)");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Serviceschedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("serviceschedules");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.AddressLine).HasMaxLength(255);
            entity.Property(e => e.ClientEmail).HasMaxLength(150);
            entity.Property(e => e.ClientFirstName).HasMaxLength(100);
            entity.Property(e => e.ClientLastName).HasMaxLength(100);
            entity.Property(e => e.ClientPhone).HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ServiceDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceTime).HasMaxLength(10);
            entity.Property(e => e.ServiceType).HasColumnType("enum('wedding','baptism','blessing','funeral')");
            entity.Property(e => e.SpecialRequests).HasColumnType("text");
        });

        modelBuilder.Entity<Serviceschedulerequirement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("serviceschedulerequirements");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.FilePath).HasMaxLength(255);
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.RequirementType).HasColumnType("enum('couple_picture','valid_id','certificate')");
            entity.Property(e => e.ServiceScheduleId).HasColumnType("int(11)");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PRIMARY");

            entity.ToTable("transactions");

            entity.HasIndex(e => e.DonationId, "DonationId");

            entity.Property(e => e.TransactionId).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreatedBy).HasColumnType("int(11)");
            entity.Property(e => e.DonationId).HasColumnType("int(11)");
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.ModifiedBy).HasColumnType("int(11)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.ReferenceNo).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pending'");

            entity.HasOne(d => d.Donation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.DonationId)
                .HasConstraintName("transactions_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.RoleId, "RoleId");

            entity.Property(e => e.UserId).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CreatedBy).HasColumnType("int(11)");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp");
            entity.Property(e => e.ModifiedBy).HasColumnType("int(11)");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnType("int(11)");

            // ✅ THESE EXIST IN YOUR DB TABLE (make EF map them)
            entity.Property(e => e.IsApproved)
                .HasColumnName("IsApproved")
                .HasColumnType("tinyint(1)")
                .HasDefaultValue(false);

            entity.Property(e => e.Status)
                .HasColumnName("Status")
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'");

            entity.Property(e => e.CompleteAddress)
                .HasColumnName("CompleteAddress")
                .HasMaxLength(255)
                .HasDefaultValue("");

            entity.Property(e => e.PhoneNumber)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(50)
                .HasDefaultValue("");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
