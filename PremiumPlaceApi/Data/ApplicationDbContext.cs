using Microsoft.EntityFrameworkCore;
using PremiumPlace_API.Models;

namespace PremiumPlace_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Place> Places { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Amenity> Amenitys { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Place>()
                .HasOne(p => p.City)
                .WithMany(c => c.Places)
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Place>()
                .Property(p => p.Rate)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Place>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Place>()
                 .HasMany(p => p.Amenitys)
                 .WithMany(a => a.Places)
                 .UsingEntity(j => j.ToTable("PlaceAmenitys"));

            modelBuilder.Entity<Place>()
                .OwnsOne(p => p.Features, b =>
                {
                    b.Property(x => x.Internet).HasColumnName("HasInternet");
                    b.Property(x => x.AirConditioned).HasColumnName("HasAirConditioned");
                    b.Property(x => x.PetsAllowed).HasColumnName("PetsAllowed");
                    b.Property(x => x.Parking).HasColumnName("HasParking");
                    b.Property(x => x.Entertainment).HasColumnName("HasEntertainment");
                    b.Property(x => x.Kitchen).HasColumnName("HasKitchen");
                    b.Property(x => x.Refrigerator).HasColumnName("HasRefrigerator");
                    b.Property(x => x.Washer).HasColumnName("HasWasher");
                    b.Property(x => x.Dryer).HasColumnName("HasDryer");
                    b.Property(x => x.SelfCheckIn).HasColumnName("SelfCheckIn");
                });

            modelBuilder.Entity<Amenity>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<Review>()
               .Property(r => r.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.PlaceId, r.UserId })
                .IsUnique();

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.PlaceId, r.CreatedAt });

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Place)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Booking>(b =>
            {
                b.HasKey(x => x.Id);    

                b.HasOne(x => x.Place)
                 .WithMany()
                 .HasForeignKey(x => x.PlaceId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.User)
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => new { x.PlaceId, x.CheckInDate, x.CheckOutDate });
                b.HasIndex(x => new { x.UserId, x.CreatedAt });
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
