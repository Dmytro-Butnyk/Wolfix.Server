using Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure;

public sealed class IdentityContext
    : IdentityDbContext<Account, Role, Guid, IdentityUserClaim<Guid>, AccountRole,
        IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public IdentityContext() { }

    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options) { }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (optionsBuilder.IsConfigured) return;
    //     
    //     var connectionString = "connection_string";
    //     optionsBuilder.UseNpgsql(connectionString);
    // }
}