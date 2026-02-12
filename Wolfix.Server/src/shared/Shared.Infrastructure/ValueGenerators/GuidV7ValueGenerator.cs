using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Shared.Infrastructure.ValueGenerators;

public sealed class GuidV7ValueGenerator : ValueGenerator<Guid>
{
    public override Guid Next(EntityEntry entry)
        => Guid.CreateVersion7();

    public override bool GeneratesTemporaryValues => false;
}