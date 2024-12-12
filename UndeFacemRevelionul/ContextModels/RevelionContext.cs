using Microsoft.EntityFrameworkCore;
using UndeFacemRevelionul.Logic;
using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.ContextModels;


public class RevelionContext : DbContext
{
    public RevelionContext(DbContextOptions<RevelionContext> options ) : base(options)  {  }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<PartyModel> Parties { get; set; }
    public DbSet<PartierModel> Partiers { get; set; }
    public DbSet<ProviderModel> Providers { get; set; }
    public DbSet<LocationModel> Locations {  get; set; }
    public DbSet<FoodMenuModel> FoodMenus {  get; set; }
    public DbSet<PlaylistModel> Playlists { get; set; }
    public DbSet<RankModel> Ranks { get; set; }
    public DbSet<SongModel> Songs { get; set; }
    public DbSet<SuperstitionModel> Superstitions { get; set; }     
    public DbSet<TaskModel> Tasks { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.Providers)   // A user can be a provider
            .WithOne(p => p.User)        // A provider has one user
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);  // On delete, cascade to provider

        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.Partiers)  // A user can be a partier
            .WithOne(p => p.User)      // A partier has one user
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);  // On delete, cascade to partier

        modelBuilder.Entity<PartierModel>()
            .HasMany(p => p.PlaylistSongs) // A partier can have many playlist songs
            .WithOne()  // Assuming PlaylistSong has a PartierId (or a Partier property)
            .HasForeignKey(ps => ps.PartierId)
            .OnDelete(DeleteBehavior.Cascade);  // On delete, cascade to playlist songs

        // You can add more configurations based on your needs (e.g., indexes, constraints)

        // Define the many-to-many relationship between Party and Partier
        modelBuilder.Entity<PartyPartierModel>()
            .HasKey(pp => new { pp.PartyId, pp.PartierId }); // Composite key

        // Define the relationship between PartyPartierModel and PartyModel
        modelBuilder.Entity<PartyPartierModel>()
            .HasOne(pp => pp.Party)
            .WithMany(p => p.PartyUsers)
            .HasForeignKey(pp => pp.PartyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Define the relationship between PartyPartierModel and PartierModel
        modelBuilder.Entity<PartyPartierModel>()
            .HasOne(pp => pp.Partier)
            .WithMany(p => p.PartyUsers)
            .HasForeignKey(pp => pp.PartierId)
            .OnDelete(DeleteBehavior.Cascade);


        // Partier and Party many-to-many relationship (handled by PartyPartierModel)
        modelBuilder.Entity<PartyPartierModel>()
            .HasKey(pp => new { pp.PartyId, pp.PartierId });

        modelBuilder.Entity<PartyPartierModel>()
            .HasOne(pp => pp.Party)
            .WithMany(p => p.PartyUsers)
            .HasForeignKey(pp => pp.PartyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PartyPartierModel>()
            .HasOne(pp => pp.Partier)
            .WithMany(p => p.PartyUsers)
            .HasForeignKey(pp => pp.PartierId)
            .OnDelete(DeleteBehavior.Cascade);

        // TaskModel to PartierModel relationship (many-to-one)
        modelBuilder.Entity<TaskModel>()
            .HasOne(t => t.Partier) // Task assigned to a Partier
            .WithMany() // No navigation property on Partier
            .HasForeignKey(t => t.PartierId)
            .OnDelete(DeleteBehavior.Cascade);

        // TaskModel to PartyModel relationship (many-to-one)
        modelBuilder.Entity<TaskModel>()
            .HasOne(t => t.Party) // Task related to a Party
            .WithMany() // No navigation property on Party
            .HasForeignKey(t => t.PartyId)
            .OnDelete(DeleteBehavior.Cascade);

        // SuperstitionModel to PartierModel relationship (many-to-one)
        modelBuilder.Entity<SuperstitionModel>()
            .HasOne(s => s.Partier) // Superstition assigned to a Partier
            .WithMany() // No navigation property on Partier
            .HasForeignKey(s => s.PartierId)
            .OnDelete(DeleteBehavior.Cascade);

        // SuperstitionModel to PartyModel relationship (many-to-one)
        modelBuilder.Entity<SuperstitionModel>()
            .HasOne(s => s.Party) // Superstition related to a Party
            .WithMany() // No navigation property on Party
            .HasForeignKey(s => s.PartyId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}