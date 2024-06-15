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
        
    }
}