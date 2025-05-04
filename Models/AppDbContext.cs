using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MobileCare.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Repairorder> Repairorders { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySQL("server=localhost;database=mobilecare;user=root;password=Enoah90(mysql);");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.ToTable("customer");

            entity.HasIndex(e => e.PhoneNumber, "PhoneNumber").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PRIMARY");

            entity.ToTable("device");

            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.DeviceCondition).HasMaxLength(100);
            entity.Property(e => e.Imei)
                .HasMaxLength(50)
                .HasColumnName("IMEI");
            entity.Property(e => e.Model).HasMaxLength(100);
        });

        modelBuilder.Entity<Repairorder>(entity =>
        {
            entity.HasKey(e => e.RepairOrderId).HasName("PRIMARY");

            entity.ToTable("repairorder");

            entity.HasIndex(e => e.CustomerId, "CustomerId");

            entity.HasIndex(e => e.DeviceId, "DeviceId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.EstimatedCost).HasPrecision(10);
            entity.Property(e => e.ProblemDescription).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Pending'")
                .HasColumnType("enum('Pending','InProgress','Ready','Collected')");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");

            entity.HasOne(d => d.Customer).WithMany(p => p.Repairorders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("repairorder_ibfk_1");

            entity.HasOne(d => d.Device).WithMany(p => p.Repairorders)
                .HasForeignKey(d => d.DeviceId)
                .HasConstraintName("repairorder_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Repairorders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("repairorder_ibfk_3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'Employee'")
                .HasColumnType("enum('Admin','Employee')");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
