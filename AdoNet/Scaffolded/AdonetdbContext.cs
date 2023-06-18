using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AdoNet.Scaffolded;

public partial class AdonetdbContext : DbContext
{
    public AdonetdbContext()
    {
    }

    public AdonetdbContext(DbContextOptions<AdonetdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=us-east.connect.psdb.cloud;user=vw3ad7aefas2b0b61pnn;database=adonetdb;port=3306;password=pscale_pw_FErHLZrXaihAdFh81Qp5iOdlaU3aDpbDHiuVaiLSMlA;sslmode=VerifyFull", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.23-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.DeleteDt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.FiredDt).HasColumnType("datetime");
            entity.Property(e => e.IdChief).HasColumnName("Id_chief");
            entity.Property(e => e.IdMainDep).HasColumnName("Id_main_dep");
            entity.Property(e => e.IdSecDep).HasColumnName("Id_sec_dep");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Secname).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.DeleteDt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeleteDt)
                .HasColumnType("datetime")
                .HasColumnName("delete_dt");
            entity.Property(e => e.ManagerId)
                .HasComment("REFERENCES Managers(Id)")
                .HasColumnName("manager_id");
            entity.Property(e => e.ProductId)
                .HasComment("REFERENCES Products(Id)")
                .HasColumnName("product_id");
            entity.Property(e => e.SaleDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("sale_date");
            entity.Property(e => e.Units)
                .HasDefaultValueSql("'1'")
                .HasColumnName("units");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
