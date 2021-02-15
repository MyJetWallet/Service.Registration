using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Service.Registration.Database
{
    public class RegistrationContext: DbContext
    {
        public const string Schema = "registration";

        public RegistrationContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        public DbSet<RegistrationEntity> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder
                .Entity<RegistrationEntity>()
                .HasKey(e => new {e.BrokerId, e.ClientId});

            base.OnModelCreating(modelBuilder);
        }
    }
}
