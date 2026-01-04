using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;
using Support.Domain.Entities;
using Support.Infrastructure.Configurations;

namespace Support.Infrastructure;

public sealed class SupportContext : DbContext, IContextWithConfigurations
{
    public SupportContext() { }

    public SupportContext(DbContextOptions<SupportContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("support");
        ApplyConfigurations(modelBuilder);
    }

    public void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SupportEntityConfiguration());
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (optionsBuilder.IsConfigured) return;
    //     
    //     optionsBuilder.UseNpgsql("");
    // }
    
    internal DbSet<Domain.Entities.Support> Supports { get; set; }
    internal DbSet<SupportRequest> SupportRequests { get; set; }
}