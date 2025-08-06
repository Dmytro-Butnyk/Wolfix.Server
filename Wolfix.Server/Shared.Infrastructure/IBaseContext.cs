using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure;

public interface IBaseContext
{
    void ApplyConfigurations(ModelBuilder modelBuilder);
}