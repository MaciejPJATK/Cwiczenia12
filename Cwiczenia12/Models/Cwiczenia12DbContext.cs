using Microsoft.EntityFrameworkCore;

namespace Cwiczenia12.Models
{
    public partial class Cwiczenia12DbContext : DbContext
    {
        public Cwiczenia12DbContext(DbContextOptions<Cwiczenia12DbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<ClientTrip> ClientTrips { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CountryTrip> CountryTrips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("Client");
                entity.HasKey(e => e.IdClient);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(120);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(120);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(120);
                entity.Property(e => e.Telephone).IsRequired().HasMaxLength(120);
                entity.Property(e => e.Pesel).IsRequired().HasMaxLength(120);
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("Trip");
                entity.HasKey(e => e.IdTrip);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(120);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(220);
                entity.Property(e => e.DateFrom).IsRequired();
                entity.Property(e => e.DateTo).IsRequired();
                entity.Property(e => e.MaxPeople).IsRequired();
            });

            modelBuilder.Entity<ClientTrip>(entity =>
            {
                entity.ToTable("Client_Trip");
                entity.HasKey(e => new { e.IdClient, e.IdTrip });
                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientTrips)
                    .HasForeignKey(d => d.IdClient);
                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.ClientTrips)
                    .HasForeignKey(d => d.IdTrip);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");
                entity.HasKey(e => e.IdCountry);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(120);
            });

            modelBuilder.Entity<CountryTrip>(entity =>
            {
                entity.ToTable("Country_Trip");
                entity.HasKey(e => new { e.IdCountry, e.IdTrip });
                entity.HasOne(d => d.Country)
                    .WithMany(p => p.CountryTrips)
                    .HasForeignKey(d => d.IdCountry);
                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.CountryTrips)
                    .HasForeignKey(d => d.IdTrip);
            });

        }
    }
}