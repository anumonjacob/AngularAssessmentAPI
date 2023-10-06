using System;
using System.Collections.Generic;
using AngularAssessmentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularAssessmentAPI.Data
{
    public partial class TableDbContext : DbContext
    {
        public TableDbContext()
        {
        }

        public TableDbContext(DbContextOptions<TableDbContext> options): base(options)
        {
        }

        public virtual DbSet<AoTable> AoTables { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("server=VM-104;database=PolAdminSys;uid=Training; pwd=May2022#;Trusted_Connection=False;TrustServerCertificate=True;MultipleActiveResultSets=true");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AoTable>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_AOTable_Id");

                entity.ToTable("AOTable");

                entity.HasIndex(e => e.Name, "ix_AOTable_Name");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Boundary).HasDefaultValueSql("((0))");
                entity.Property(e => e.Cache).HasDefaultValueSql("((0))");
                entity.Property(e => e.Comment).HasMaxLength(2048);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.History).HasDefaultValueSql("((0))");
                entity.Property(e => e.Identifier).HasDefaultValueSql("((0))");
                entity.Property(e => e.Log).HasDefaultValueSql("((0))");
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Notify).HasDefaultValueSql("((0))");
                entity.Property(e => e.Premium).HasColumnName("premium");
                entity.Property(e => e.Type).HasMaxLength(128);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}