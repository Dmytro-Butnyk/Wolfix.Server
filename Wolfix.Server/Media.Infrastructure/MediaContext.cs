using Media.Domain.BlobAggregate;
using Media.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Media.Infrastructure;

public sealed class MediaContext : DbContext
{
    public MediaContext() { }
    
    public MediaContext(DbContextOptions<MediaContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ApplyConfigurations(modelBuilder);
        
        modelBuilder.HasDefaultSchema("media");
    }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (optionsBuilder.IsConfigured) return;
    //     
    //     var connectionString = "";
    //     optionsBuilder.UseNpgsql(connectionString);
    // }
    
    public void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BlobResourceEntityConfiguration());
    }
    
    internal DbSet<BlobResource> BlobResources { get; set; } // Aggregate 
}