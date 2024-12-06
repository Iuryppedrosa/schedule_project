using Microsoft.EntityFrameworkCore;
using scheduler.Models.Entities;

namespace scheduler.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Court> Courts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Guid).IsUnique();
                entity.Property(e => e.FederalId).HasMaxLength(20);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Contact).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
            });


            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired(false);
                entity.Property(e => e.Title).HasMaxLength(255).IsRequired(false);
                entity.Property(e => e.CourtId).IsRequired(false);
                entity.Property(e => e.Guid)
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(e => e.UserFederalId)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.UserGuid)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(e => e.Details)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate)
                    .IsRequired();

                entity.Property(e => e.CreatedDate)
                    .IsRequired();

                entity.Property(e => e.DeletedDate)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Court>(entity =>
            {
                entity.ToTable("Court");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Guid)
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(c => c.Name)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(c => c.Active)
                    .IsRequired();

                entity.Property(c => c.Place)
                    .HasMaxLength(255)
                    .IsRequired(false);

                entity.Property(c => c.CreatedDate)
                    .IsRequired();

                entity.Property(c => c.DeletedDate)
                    .IsRequired(false);
            });


        }

        
    }
}
