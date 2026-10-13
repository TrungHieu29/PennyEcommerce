using Microsoft.EntityFrameworkCore;
using PennyEcommerce.Models;
using PennyEcommerce.Models.Auth;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<AppRole> Roles => Set<AppRole>();
    public DbSet<AppUserRole> UserRoles => Set<AppUserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.NormalizedEmail)
            .IsUnique();
        modelBuilder.Entity<AppRole>()
            .HasIndex(r => r.NormalizedName)
            .IsUnique();

        modelBuilder.Entity<AppUserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<AppUserRole>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<AppUserRole>()
            .HasOne<AppRole>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId);
    }
}