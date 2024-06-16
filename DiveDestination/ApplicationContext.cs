using Microsoft.EntityFrameworkCore;
using Server_Test.Models;

namespace DiveDestination;

public class ApplicationContext: DbContext
{
    public DbSet<Persons> Persons { get; set; } = null!;
    public DbSet<PassData> PassData { get; set; } = null!;
    public DbSet<UserLevel> UserLevel { get; set; } = null!;
    public DbSet<Posts> Posts { get; set; } = null!;
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связи один-к-одному для PassData
        modelBuilder.Entity<Persons>()
            .HasOne(p => p.PassData)
            .WithOne(d => d.Persons)
            .HasForeignKey<Persons>(p => p.passport)
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка связи один-к-одному для UserLevel
        modelBuilder.Entity<Persons>()
            .HasOne(p => p.UserLevel)
            .WithOne(u => u.Person)
            .HasForeignKey<Persons>(p => p.user_level)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Настройка связи один-к-одному для UserLevel
        modelBuilder.Entity<UserLevel>()
            .HasOne(p => p.Roles)
            .WithOne(u => u.UserLevel)
            .HasForeignKey<UserLevel>(p => p.level)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}