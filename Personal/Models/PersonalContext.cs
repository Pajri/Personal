using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Personal.Models
{
    public partial class PersonalContext : DbContext
    {
        public virtual DbSet<PersonalPost> PersonalPost { get; set; }

        public PersonalContext(DbContextOptions<PersonalContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonalPost>(entity =>
            {
                entity.ToTable("Personal_Post");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.InsertDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });
        }
    }
}
