using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure;

public interface IContextWithConfigurations
{
    void ApplyConfigurations(ModelBuilder modelBuilder);
}