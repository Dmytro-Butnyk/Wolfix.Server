using Admin.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;

namespace Admin.Infrastructure;

public sealed class AdminContext : DbContext, IContextWithConfigurations
{
    public AdminContext() { }
    
    public AdminContext(DbContextOptions<AdminContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("admin");
        ApplyConfigurations(modelBuilder);
    }

    public void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AdminEntityConfiguration());
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (optionsBuilder.IsConfigured) return;
    //     
    //     optionsBuilder.UseNpgsql(connectionString);
    // }
    
    public DbSet<Domain.AdminAggregate.Admin> Admins { get; set; }
}