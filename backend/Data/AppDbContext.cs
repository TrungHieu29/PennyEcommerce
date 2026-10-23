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
    public DbSet<AppRefreshToken> RefreshTokens => Set<AppRefreshToken>();

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

        modelBuilder.Entity<AppRefreshToken>()
            .HasIndex(token => token.TokenHash)
            .IsUnique();

        modelBuilder.Entity<AppRefreshToken>()
            .HasIndex(token => token.UserId);

        modelBuilder.Entity<AppRefreshToken>()
            .Property(token => token.TokenHash)
            .HasMaxLength(64)
            .IsRequired();

        modelBuilder.Entity<AppRefreshToken>()
            .HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.IsDeleted);
    }
}
