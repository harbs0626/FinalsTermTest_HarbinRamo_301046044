using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Finals_WPF.Models
{
    public partial class COVID19DeathsGlobalDBContext : DbContext
    {
        public COVID19DeathsGlobalDBContext()
        {
        }

        public COVID19DeathsGlobalDBContext(DbContextOptions<COVID19DeathsGlobalDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Covid19deaths> Covid19deaths { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=COVID19DeathsGlobalDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Covid19deaths>(entity =>
            {
                entity.HasKey(e => new { e.CountryRegion, e.ProvinceState, e.RecordDate })
                    .HasName("PK_ConfirmCase");

                entity.ToTable("COVID19Deaths");

                entity.Property(e => e.CountryRegion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceState)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RecordDate).HasColumnType("date");

                entity.Property(e => e.DeathNumber).HasDefaultValueSql("((0))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
