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
        public DbSet<Scheduler> Schedulers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Guid).IsUnique();
                entity.Property(e => e.FederalId).HasMaxLength(20);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Configuração da entidade Scheduler
            /*modelBuilder.Entity<Scheduler>(entity =>
            {
                entity.ToTable("Schedulers");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Guid).IsUnique();

                // Relacionamento com User
                entity.HasOne(s => s.User)
                     .WithMany()
                     .HasForeignKey(s => s.UserId)
                     .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.UserFederalId).HasMaxLength(20);
                entity.Property(e => e.Details).IsRequired().HasMaxLength(500);
            });*/
        }

        
    }
}
