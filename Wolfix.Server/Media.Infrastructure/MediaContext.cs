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
        ApplyConfigurations(modelBuilder);
    }
    
    public void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BlobResourceEntityConfiguration());
    }
    
    internal DbSet<BlobResource> BlobResources { get; set; } // Aggregate 
}